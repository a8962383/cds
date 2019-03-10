#!/bin/bash

set -x

if [ -z "$NUMBER_CPU" ]; then
      NUMBER_CPU="1,8"
fi

if [ -z "$BUILD_TAG" ]; then
      BUILD_TAG="notag"
fi

if [ ! -z "$USERNAME" ]; then
      echo "$USERNAME"
fi

cd $HOME

# Stop and remove old container
CONTAINER=$(docker ps -a --no-trunc | grep '$USERNAME' | awk '{ print $1 }' | tr '\n' ' ')
if [ ! -z "$CONTAINER" ]; then
     docker rm --force $CONTAINER
fi

# Remove old container image
IMAGE=$(docker images | grep '$USERNAME' | awk '{ print $3 }' | tr '\n' ' ')
if [ ! -z "$IMAGE" ]; then
     docker rmi $IMAGE
fi

# Stop and remove old container
CONTAINER=$(docker ps -a --no-trunc | grep "$USERNAME" | awk '{ print $1 }' | tr '\n' ' ')
if [ ! -z "$CONTAINER" ]; then
     docker rm --force $CONTAINER
fi

# Remove old container image
IMAGE=$(docker images | grep "$USERNAME" | awk '{ print $3 }' | tr '\n' ' ')
if [ ! -z "$IMAGE" ]; then
     docker rmi $IMAGE
fi

cd /data/cdslab/$USERNAME
docker build -t $USERNAME . > docker.$BUILD_TAG.log
IMAGE=$(grep 'Successfully built' docker.$BUILD_TAG.log | awk '{ print $3 }')


if [ ! -z "$IMAGE" ]; then
    set -e
    # replace cds-tool in student image

    # the subsequent `docker commit` replaces the image's CMD
    # thus we extract the orignal CMD here
    CMD=`docker inspect -f {{.ContainerConfig.Cmd}} ${USERNAME} | sed -e 's/.* \"-c\" \"\(.*\)\"]]/\1/g'`
    # replace cds-tool inside the container
    CONTAINER=$(docker run -v $(pwd)/cds-root/cds-tool/bin/:/mnt -d $USERNAME bash -c 'eval "cp /mnt/cds-tool `which cds-tool`"')
    # wait for cp to finish
    docker wait ${CONTAINER}
    ORIG_IMAGE=${IMAGE}
    # turn the container into an image and tag it with the student's username
    IMAGE=`docker commit -c "CMD ${CMD}" ${CONTAINER} ${USERNAME}`
    # remove the container
    docker rm ${CONTAINER}
    set +e

    CDS_TOOL=`realpath ./cds-root/cds-tool/bin/cds-tool`
    EXP="t5-shortest-superstring"
    
    pushd /data/cdslab/$USERNAME
        # check results
        #$CDS_TOOL run --image $USERNAME -r 1 -c 1 -i ./cds-root/mopp/$EXP/test.in -o ./cds-root/mopp/$EXP/test.out mopp-$EXP

        # run experiments
        #$CDS_TOOL run --measure --image $USERNAME -r 1 -c $NUMBER_CPU -i ./cds-root/mopp/$EXP/judge.in -o ./cds-root/mopp/$EXP/judge.out mopp-$EXP | tee $EXP.$BUILD_TAG.log
        $CDS_TOOL run --measure --image $USERNAME -r 1 -c $NUMBER_CPU -i ./cds-root/mopp/$EXP/judge.in mopp-$EXP | tee $EXP.$BUILD_TAG.log

        python ./cds-root/evaluate.py $EXP.$BUILD_TAG.log | tee speedup.$BUILD_TAG.log
    popd
fi

CONTAINER=$( docker ps -a --no-trunc | grep '$USERNAME' | awk '{ print $1 }' | tr '\n' ' ')
if [ ! -z "$CONTAINER" ]; then
    docker rm --force $CONTAINER
fi


IMAGE=$(docker images | grep '$USERNAME' | awk '{ print $3 }' | tr '\n' ' ')
if [ ! -z "$IMAGE" ]; then
	 docker rmi $IMAGE
     docker rmi $ORIG_IMAGE || true
     # we had some issues with storage space, therefore we remove all removable (unused) images here
     docker rmi `docker images -q` || true
fi


CONTAINER=$( docker ps -a --no-trunc | grep "$USERNAME" | awk '{ print $1 }' | tr '\n' ' ')
if [ ! -z "$CONTAINER" ]; then
    docker rm --force $CONTAINER
fi


IMAGE=$(docker images | grep "$USERNAME" | awk '{ print $3 }' | tr '\n' ' ')
if [ ! -z "$IMAGE" ]; then
	 docker rmi $IMAGE
     docker rmi $ORIG_IMAGE || true
     # we had some issues with storage space, therefore we remove all removable (unused) images here
     docker rmi `docker images -q` || true
fi
