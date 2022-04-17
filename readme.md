# Car Load Optimizer

## Requirements

This app was build with .NET 6.0. 

It can be downloaded at https://dotnet.microsoft.com/en-us/download/dotnet/6.0 (just the SDK is needed)

## How to run

Executable contained in CarLoadOptimizer\bin\Release\net6.0\publish

The folder also contains an example input file

To run, open a console terminal in this folder and run this command with the two following parameters:
> CarLoadOptimizer path_to_input_file.json path_to_output_file.json
- path_to_input_file.json: path to your json input file containing the charging query
- path_to_output_file.json: path to your json output file where you want to get the result

## Possible errors:

The program will return errors if the following parameters are invalid:
- Input file not existing or not of JSON format
- Output file not of JSON format (will be created if it does not exists)

Also if the following JSON input file content are wrong:
- Car data missing or wrong (charge power and battery capacity <= 0, Current battery level <0, current battery level > battery capacity)
- Tariffs missing or wrong (energy price <=0, also if tariff intersect with each other, EG a tariff from 8 to 12 and one from 10 to 14)
- User settings missing or wrong (no tariffs specified, direct charging percentage or desired charging percentage < 0, direct charging percentage > desired charging percentage)
- Charging query missing or wrong (JSON file empty, no car data / user settings specified, starting charge time in the past, if tariff do not cover the full charging period)

## Project structure

Each .cs class contains comments with additional information

-- CarLoadOptimizer: Main folder, containing the app  
--- CLoException: Folder containing custom app exception  
---- JsonParsingException.cs: Exception thrown on json parsing error  
---- JsonValidationException.cs: Exception thrown on json validation error  
---- NotJsonFileException.cs: Exception thrown if paramaters are not JSON files  
--- Data: Folder containg data classes  
---- Generic: Generic classes that are used in the app  
----- CustomEnum.cs: Contain custom enums  
----- TariffPeriod.cs: Tariff period with datetime start and end  
----- TimePeriod.cs: Period with TimeOnly start and end  
---- Input: JSON input classes  
----- CarData.cs: Car data: charge power, battery capacity and current battery level  
----- ChargingQuery.cs: Charging query: Starting time, user settings and car data  
----- Tariff.cs: Tariff: start time, end time and energy price  
----- UserSettings.cs: User settings: Desired and direct charge percentage, leaving time and list of tariffs  
---- Output: JSON output classes  
----- ChargingPeriod.cs: Output JSON class representing if the car is charging during a period of time  
--- Middleware: Folder containing business logic  
---- Conversion  
----- TimeOnlyConverter.cs: Custom JSON convertor that allows parsing of strings to TimeOnly  
---- DataValidation  
----- ChargingQueryValidator.cs: Validator for the charging query (checks that all data is valid)  
---- LoadOptimization  
----- LoadOptimizer.cs: Charging query optimizer. Used to retrieve the best possible charging schedule.  
---- Parsing  
----- JsonParser.cs: JSON file Reader/Writer  
--- CarLoadOptimizerMain.cs: Main running class, run on app start  

-- CarLoadOptimizerTest: Folder containing the tests  
--- DataValidation  
---- ChargingQueryValidatorTests.cs: Tests for the charging query validator middleware  
--- LoadOptimization  
---- LoadOptimizerTests.cs: Tests for the load optimizer middleware  

## Design choices
Some questions / answers on the app design choices

> Why a console app and not an API or an app with an UI?

The exercise was already quite complete, adding an UI would have unnecessarily complicated the app. 
A console app allows to answer to the requirements, and we can chain the commands pretty easily for bulk file processing if necessary.
The assignment did not specifiy that a lot of files query had to be satisfied at once, so an API was not necessary in my opinion.
Also a console app is the simplest kind of app to develop, no reason to build anything more complicated if this is enough.

> Why did you separate the app logic this way?

I created one main folder for the app, and another for the unit testing (with Nunit)

For the main app, I created:
- one folder for the custom app exceptions
- one folder for the data structures definition
- one folder for the business logic (middleware)

For testing, each test file tests one type of business logic (middleware) to check if is working as it should.
