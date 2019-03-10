.PHONY = all clean

all : shortest_superstring

clean:
	rm -rf shortest_superstring

shortest_superstring: shortest_superstring.cc
	$(CXX) -std=c++11 -Wall -O3 shortest_superstring.cc -o shortest_superstring

