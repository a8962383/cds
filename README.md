# Concurrent and Distributed Systems #

Problem Set:
12th Marathon of Parallel Programming Contest [WSCAD-2016](http://lspd.mackenzie.br/marathon/17/problems.html)

Sequential code (c/c++) and judge input/output taken from WSCAD-2016.

As you will see in the following we use Docker containers to restrict the number of CPU cores available to our programs.
Use the environment variable `MAX_CPUS` to determine the number of availabe CPU core programatically.
This variable is set by the testing tool that controls the container creation as well.
Other means, since we're running in a container, might return the total amount of CPUs of the machine and not the cores available to the container.

# How to get started #

These steps build the docker container and run concurrency tests.
In this mode the program is executed a number of times with different amounts of CPUs cores.

1. Create the Docker image:

    During the creation of the docker image the task solution programs will be build and included in the resulting image

        $ docker build . -t cds-lab:latest

    After this command succeeds you have to reference the created image. Either you use `docker build`'s `-t` option to tag (name)
    the image or you carefully eyeball for a line at the end of its output that prints the image's id, e. g.:

        `Successfully built 2e32ab3296ea`

2. Trigger the experiments with the `cds-tool` binary:

        $ ./cds-tool/bin/cds-tool run --measure --image [IMAGE REFERENC] -c [NUMBER OF CPUs] --input [INPUT FOR THE TASK] [NAME OF TASK as is cds-tool/cds_server.json]
        $ ./cds-tool/bin/cds-tool run --measure --image cds-lab:latest -c 1,2,3,4 --input mopp/t0-harmonic-progression-sum/judge.in mopp-t0-harmonic-progression-sum

    This is basically what our evaluation system does with yout image. So make sure your image works as expected.

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

# Benchmarking Tool #

For evaluating the performance of this solutions I have used an excellent benchmarking tool developed by Franz using rust. See details [here](https://github.com/fzgregor/cds-lab-2017)
