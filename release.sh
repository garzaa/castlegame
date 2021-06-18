alias butler="D:/Program\ Files/butler-windows-amd64/butler.exe"

for i in webgl; do
	butler push ./demos/ForestKeepMirror-$i sevencrane/forest-keep-mirror:$i
done
