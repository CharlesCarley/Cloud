# Running in Linux

In order to run in a Linux environment the [dotnet tools](https://docs.microsoft.com/en-us/dotnet/core/install/linux) need to be installed. 

The curren source uses `.NET Core 3.1`, and has been tested with Debian instructions.

## Building 

Clone a copy of this repo to a working directory and issue the following commands.

```
cd Cloud
chmod +x storectl
./storectl build
./storectl run
```

This will start the server on localhost port 5000. The `--cfg <path>` argument  will allow specifying different addresses.

The configuration file is formatted as JSON and expects a Host, and Port entry.


```.json
{
    "Host": "127.0.0.1",
    "Port": 5000
}
```
