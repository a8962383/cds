#!/bin/bash

# This script installs all dependencies we have for building
# as well as running our software. The idea being that no other
# installation steps are necessary after this script ran.

# have to update repository database, since image is distributed
# with an empty database
apt-get update
# for (simple) c and c++ there is a single metapackage that is
# needed:
apt-get install -y build-essential

# For .NET Core installation, these pre-requisites needs to be installed and added
apt-get update
apt-get install -y wget
wget -q packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb

# For .NET Core application, these need to be installed
apt-get update
apt-get install -y apt-transport-https
apt-get update
apt-get install -y dotnet-sdk-2.1.200