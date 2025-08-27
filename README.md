# ToLazyToCurl
Simple WPF app for playing with REST APIs and viewing responses.

> *Why bother with anything actually good, when you can make your own version?*

<br>

![Dark and Light theme](/images/App.png)

## Features
* Supports `GET`, `POST`, `PATCH`, `PUT` and `DELETE` requests
* Optional custom HTTP headers
* Dynamic syntax highlighting (JSON, XML, HTML, etc.)
* Responsive design with animations for messages and controls
* Dark/light mode
* Logging
* Restores last session on startup

## Screenshots

Split View
![Split View](/images/SplitView.png)

Error Examples
![Error Examples](/images/UiErrorMessages.png)

## Try It Out?

<details>
<summary>Run from source</summary>  

#### Requirements: 
* Windows  
* [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet)

``` bash
git clone "https://github.com/Peppson/to-lazy-to-curl.git" &&
cd "to-lazy-to-curl" &&
dotnet publish -c Release -r win-x64 --self-contained true -o "$HOME/Desktop"
```

</details> 

<details> 
<summary>Download prebuilt</summary>

#### Requirements:

* A computer! (Windows)

Grab the latest release from the [Releases tab](https://github.com/Peppson/to-lazy-to-curl/releases)  

</details>

<br>
