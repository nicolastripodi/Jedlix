# Car Load Optimizer
## How to run
Executable contained in CarLoadOptimizer\bin\Release\net6.0\publish
To run, open a console terminal in this folder and run this command with the two following parameters:
> CarLoadOptimizer path_to_input_file.json path_to_output_file.json
- path_to_input_file.json: path to your json input file containing the charging query
- path_to_output_file.json: path to your json output file where you want to get the result

## Project structure

Each .cs class contains comments with additional information

- CarLoadOptimizer: Main folder, containing the app
-- CLoException: Folder containing custom app exception
--- JsonParsingException.cs: Exception thrown on json parsing error
--- JsonValidationException.cs: Exception thrown on json validation error
--- NotJsonFileException.cs: Exception thrown if paramaters are not JSON files
-- Data: Folder containg data classes
--- Generic: Generic classes that are used in the app
---- CustomEnum.cs: Contain custom enums
---- TariffPeriod.cs: Tariff period with datetime start and end
---- TimePeriod.cs: Period with TimeOnly start and end
--- Input: JSON input classes
---- CarData.cs: Car data: charge power, battery capacity and current battery level
---- ChargingQuery.cs: Charging query: Starting time, user settings and car data
---- Tariff.cs: Tariff: start time, end time and energy price
---- UserSettings.cs: User settings: Desired and direct charge percentage, leaving time and list of tariffs
--- Output: JSON output classes
---- ChargingPeriod.cs: Output JSON class representing if the car is charging during a period of time
-- Middleware: Folder containing business logic
--- Conversion
---- TimeOnlyConverter.cs: Custom JSON convertor that allows parsing of strings to TimeOnly
--- DataValidation
---- ChargingQueryValidator.cs: Validator for the charging query (checks that all data is valid)
--- LoadOptimization
---- LoadOptimizer.cs: Charging query optimizer. Used to retrieve the best possible charging schedule.
--- Parsing
---- JsonParser.cs: JSON file Reader/Writer
-- CarLoadOptimizerMain.cs: Main running class, run on app start

- CarLoadOptimizerTest: Folder containing the tests
-- DataValidation
--- ChargingQueryValidatorTests.cs: Tests for the charging query validator middleware
-- LoadOptimization
--- LoadOptimizerTests.cs: Tests for the load optimizer middleware

## Design choices
Some questions / answers on the app design choices

> Why not use an UI?

The exercise was already quite complete, adding an UI would have unnecessarily complicated the app. A console app allows to answer to the requirements, and we can chain the commands pretty easily for bulk file processing if necessary.

> Then why not an API instead?

Again, it would have unnecessarily complicated the app. The assignment did not specifiy that a lot of files query had to be satisfied at once, so again the console app statisfy the requirements.

> Why a console app and not anything else?

Simplest kind of app to develop, no reason to build anything more complicated if this is enough.

> Why did you separate the app logic this way?

I created one main folder for the app, and another for the unit testing (with Nunit)
For the main app, I created:
- one folder for the custom app exceptions
- one folder for the data structures definition
- one folder for the business logic (middleware)

For testing, each test file tests one type of business logic (middleware) to check if is working as it should.
