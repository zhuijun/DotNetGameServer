set DIR=..
set OUT_DIR=..\ts
protoc.exe -I=%DIR% ClientServer.proto --js_out=import_style=commonjs,binary:%OUT_DIR% --grpc-web_out=import_style=typescript,mode=grpcwebtext:%OUT_DIR%