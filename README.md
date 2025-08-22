# In progress...

``` bash
git clone "https://github.com/Peppson/to-lazy-to-curl.git" &&
cd "to-lazy-to-curl" &&
dotnet publish -c Release -r win-x64 --self-contained true -o "$HOME/Desktop" && 
cd .. &&
echo -e "\n\033[0;32m> Publish complete! Output EXE is in $HOME/Desktop\033[0m"
```