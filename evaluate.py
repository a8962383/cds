import sys
import csv

#EXPECTED_RUNS = 5

# proudly copy-pasted from https://stackoverflow.com/questions/24101524/finding-median-of-list-in-python
def median(lst):
    lst = sorted(lst)
    n = len(lst)
    if n < 1:
            return None
    if n % 2 == 1:
            return lst[n//2]
    else:
            return sum(lst[n//2-1:n//2+1])/2.0

def evaluate_file(fname):
    with open(fname, 'rb') as csvfile:
        reader = csv.reader(csvfile, delimiter=';')
        data = {}
        runs = 0
        name = fname
        for row in reader:
            if len(row) != 5 and len(row) != 4:
                break
            if not row[2].strip().isdigit():
                continue
            name = row[0]
            cores = row[2].strip()
            time = int(row[3])
            if not data.has_key(cores):
                data[cores] = []
            data[cores].append(time)

        single = 0
        if data.has_key("1"):
            single = median(data["1"])
            del data["1"]
        print name, "single-threaded median time:", single, "msec"

        for (cores, times) in data.items():
            print "{} {}-core median time: {} msec speedup: {}".format(name, cores, median(times), float(single)/median(times))

if __name__ == "__main__":
    for x in sys.argv[1:]:
        evaluate_file(x)
