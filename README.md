# NiMotionTool
NiMotionTool



## Embedded Windows Python engine

download  [python-3.9.8-embed-amd64](https://www.python.org/ftp/python/3.9.8/python-3.9.8-embed-amd64.zip) and [get-pip.py](https://bootstrap.pypa.io/get-pip.py)

install pip and set environment.

Unzip python-3.9.8-embed-amd64.zip and copy get-pip.py to python-3.9.8-embed-amd64.

Remove the line of #import site Remove comment symbols# in python39._pth file.

install pip for  python-3.9.8-embed-amd64 as follows:

```shell
> cd python-3.9.8-embed-amd64
> python.exe get-pip.py
```

## Install MATLAB Engine API

Install the corresponding python matlab engine version according to the version of matlab.

```shell
pip install matlabengine==MatlabPythonEngineVersion
```

| MatlabVersion         | MatlabEngineVersion | MatlabPythonEngineVersion |
| --------------------- | ------------------- | ------------------------- |
| MATLAB release R2020b | 9.9                 | 9.9.4                     |
| MATLAB release R2021a | 9.10                | 9.10.3                    |
| MATLAB release R2021b | 9.11                | 9.11.21                   |
| MATLAB release R2022a | 9.12                | 9.12.19                   |
| MATLAB release R2022b | 9.13                | 9.13.8                    |
| MATLAB release R2023a | 9.14                | 9.13.2                    |



