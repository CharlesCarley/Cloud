# Running in Linux

In order to run in a Linux environment the [dotnet tools](https://docs.microsoft.com/en-us/dotnet/core/install/linux) need to be installed. 

The current source uses `.NET Core 3.1`.

## Building 

Clone a copy of this repo to a working directory and issue the following commands.

```
cd Cloud
chmod +x storectl
./storectl build
./storectl run
```

This will start the server on localhost port 5000. 

The `--cfg <path>` argument will allow changing the address.
It is formatted as JSON and expects a Host, and Port entry.

```.json
{
    "Host": "127.0.0.1",
    "Port": 5000
}
```

Depending on where it is running it can be tested in the browser by going to the address. `http://127.0.0.1:5000/ping`

The output should be:
```
Cloud.Store running successfully
```
