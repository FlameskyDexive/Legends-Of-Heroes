cd /d Tools/Luban
call gen_code_client_server.bat Localhost
call gen_code_client.bat Localhost
call gen_code_server.bat Localhost

call gen_code_client_server.bat Release
call gen_code_client.bat Release
call gen_code_server.bat Release

call gen_code_client_server.bat RouterTest
call gen_code_client.bat RouterTest
call gen_code_server.bat RouterTest

call gen_code_client_server.bat Benchmark
call gen_code_client.bat Benchmark
call gen_code_server.bat Benchmark
pause