set -x
set -e

docker build . -t cds-lab

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t0-harmonic-progression-sum/test.in --output mopp/t0-harmonic-progression-sum/test.out mopp-t0-harmonic-progression-sum

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t1-transitive-closure/test.in --output mopp/t1-transitive-closure/test.out mopp-t1-transitive-closure

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t2-eternity/test.in --output mopp/t2-eternity/test.out mopp-t2-eternity

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t3-mandelbrot-set/test.in --output mopp/t3-mandelbrot-set/test.out mopp-t3-mandelbrot-set

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t4-k-means-clustering/test.in --output mopp/t4-k-means-clustering/test.out mopp-t4-k-means-clustering

./cds-tool/bin/cds-tool run --measure --image cds-lab -r 1 -c 1 --input mopp/t5-shortest-superstring/test.in --output mopp/t5-shortest-superstring/test.out mopp-t5-shortest-superstring
