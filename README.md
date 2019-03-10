# Concurrent and Distributed Systems Lab: Summer semester 2018 #

Problem Set:
12th Marathon of Parallel Programming Contest [WSCAD-2016](http://lspd.mackenzie.br/marathon/17/problems.html)

Sequential code (c/c++) and judge input/output taken from WSCAD-2016
Some minor bug of sequential code is fixed (eg: game-of-life input reading, histogram input format)

As you will see in the following we use Docker containers to restrict the number of CPU cores available to you program.
Use the environment variable `MAX_CPUS` to determine the number of availabe CPU core programatically.
This variable is set by our testing tool that controls the container creation as well.
Other means, since you're running in a container, might return the total amount of CPUs of the machine and not the cores available to the container.

# Benchmarking Tool #

For evaluating the performance of this solutions I have used an excellent benchmarking tool developed by Franz using rust. See details [here](https://github.com/fzgregor/cds-lab-2017)

# About this Repository #

The `mopp` directory contains sub directories resembling sequential solutions of 2017's marathon tasks.
Everything is built via Makefiles.
There is a global Makefile in `mopp` and task-specific ones in their respective sub directories.
The global Makefile is invoked in `build.sh` which gets called during the Docker image creation (see `Dockerfile`).

In `cds-tool` you'll find the source code of the program that 1) creates a server inside of the running Docker container and invokes your programs and 2) queries this server from the outside and starts the docker container if necessary.
A precompiled binary can be found at `cds-tool/bin/cds-tool`. You can recompile the tool with `build_cds-tool.sh`. This script will install [Rust](https://www.rust-lang.org/) on your machine if it's not already installed. (You can run it in an container if you don't want Rust to be installed on your machine ;) )

The file `mopp/cds_server.json` contains a lookup table for the paths to the binaries of the tasks used by the server.
Change the paths if your setup puts the binaries to other locations (which is likely).
Update `mopp/cds_server.json` to reflect the situation in your image.
It links program names to the executable's location INSIDE of the image.
This allows the server to invoke the correct program.
For example with an entry of `["mopp-histogram", "/mopp/histogram/histogram"], ...` the
server will invoke `/mopp/histogram/histogram` when the client requests `mopp-histogram`
to be executed.
The four tasks have to be named as follows for our script to invoke the correct program:

* mopp-t0-harmonic-progression-sum 
* mopp-t1-transitive-closure
* mopp-t2-eternity
* mopp-t3-mandelbrot-set
* mopp-t4-k-means-clustering
* mopp-t5-shortest-superstring

# General Advice #

* Develop your software your way. Use tools, languages, and libraries as you wish.
* Adapt the image building process such that
  a) runtime and build dependencies are available in the generated image and
  b) your software is built during image creation.
  Have a look at `Dockerfile`, `install_deps.sh` and `build.sh` to understand how we did it in this 
  template. We recommend you a comparable separation as this eases development.
  Make sure the CDS server is started by default (see `CMD` line in `Dockerfile`).
  *Otherwise, automatic measuring will fail.*
* If the container was started by the `cds-tool` it's environment will contain `MAX_CPUS` which you can read
  in your program to learn how many cpus are availabe for it.

## Development within an Container ##

During development you should also use the container environment.
This might be cumbersome at first but will reduce the pain near the deadline.

Start by creating an interactive container of the base image used in your `Dockerfile`, mount the
cds repository where it would be copied to by the `Dockerfile` and expose port 8080 of the container.
For our seqeuntial template repository this would be done like this (assuming we are currently
in the repositories root directory):


```
#!bash

$ docker run -it --rm -v `pwd`:/cds-lab -p8080 ubuntu:16.04 /bin/bash
```


The container starts and you have an interactive bash session on the inside. Please note the `--rm`
argument which instructs the Docker engine to remove the container once its stopped. Make sure your
code changes are not removed with the container ;).

Now, you can manually execute the two setup steps of the `Dockerfile` inside your interactive container:
Install your dependencies:

```
#!bash

$ /cds-lab/install_deps.sh
```

and build your software:

```
#!bash

$ pushd cds-lab && ./build.sh && popd
```


With this done you can start the CDS server:

```$ /cds-lab/cds-tool/bin/cds-tool server -c cds-lab/cds-tool/cds_server.json```

The CDS server is waiting for requests now. So you can switch to a console on your host and invoke
the CDS measurement tool:

* Find the container's name or id with docker ps:

```
#!bash

   $ docker ps 
   CONTAINER ID        IMAGE               COMMAND             CREATED              STATUS              PORTS                     NAMES
   3979aea71b8e        ubuntu:16.04        "/bin/bash"         About a minute ago   Up About a minute   0.0.0.0:32778->8080/tcp   tender_stonebraker
```


* Invoke the tool normally:

```
#!bash

   ./cds-tool/bin/cds-tool run --container tender_stonebraker --cpus 2 -i ./mopp/sudokount/sudokount1.in mopp-sudokount
   ran program mopp-sudokount
   exit status: 0
   duration: 643 micro seconds
   stdout:
   --------------
   1
   --------------
   stderr:
   --------------
   --------------
```


Now you can alter your code, stop the server, invoke the build script, restart the server and retry.
You can omit the hassle of stopping and restarting the server if you get yourself another bash
session in the container:

```$ docker exec -it $CONTAINER_ID_OR_NAME /bin/bash```

This creates another bash session inside of the container which you can use to reinvoke the build script.

One last hint: If you encounter issues with the way your program is run there are two options:

* Activate debug or even trace output of the CDS server and client to see in more detail what is going on:

```
#!bash

$ RUST_LOG=trace ./cds-tool/bin/cds-tool run --container tender_stonebraker --cpus 2 -i ./mopp/sudokount/sudokount1.in mopp-sudokount
$ RUST_LOG=debug /cds-lab/cds-tool/bin/cds-tool server -c cds-lab/cds-tool/cds_server.json
```


   This will help you understand what these tools do in more detail.

* Obviously, you can run your program without the CDS server and client allowing you to verify the
   correct function of your program. Running sudokount inside of the container:
   
```
#!bash

$ /cds-lab/mopp/sudokount/sudokount < /cds-lab/mopp/sudokount/sudokount2.in 
     300064
```
