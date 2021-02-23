::Installation
::$ npm i -g csvtojson

@echo off
	for /F %%i in ('Dir Csv\*.csv /B') do ( csvtojson Csv/%%~ni.csv > Json/%%~ni.json  )
pause