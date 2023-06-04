#! /bin/bash

printf "Packaging...\n"
cd MontyHall && dotnet lambda package --configuration Release --framework net6.0 --output-package bin/MontyHall.zip && cd ..; \

printf "Done packaging\n"
printf "Start deploying...\n"
sls deploy --verbose --config serverless.yml

printf "Done deploying\n"
