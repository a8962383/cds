FROM ubuntu:16.04


# copy the cds-tool such that it is in the search PATH
COPY cds-tool/bin/cds-tool /usr/local/bin

# install your dependencies
COPY install_deps.sh /cds-lab/install_deps.sh
RUN /cds-lab/install_deps.sh

# copy over your source code & cds server configuration
COPY . /cds-lab

# build your programs
RUN cd cds-lab && ./build.sh

# start the cds server by default
# make sure it reads your configuration
CMD cds-tool server -c /cds-lab/cds-tool/cds_server.json
