# Research Dashboard

## Overview
This project was done as part of the Honours programme for the Computer Science bachelor at Utrecht University.
It includes one program, consisting of three components:
1. Filter: responsible for taking a data set containing meta data about research publications and parsing this to extract all the data that we are interested in to fill a json file. This native json file follows the same format for each data set that can be parsed. Currently the supported data sets are: DBLP, PubMed and PURE.
2. Importer: responsible for taking a native json file that the filter outputs and reading it into datastructures in memory, adding additional data where necessary. This additional data can be extracted from the pdf of the respective publication or from other external sources. This component also provides a way to extract all of the data currently in memory to a json file, which importantly does not follow the same format as the native json file, and also a way to import such a file back into memory, overwriting any data that was already there.
3. API: allows the data that is stored in memory (as imported by the Importer) to be queried using a simplistic, self-defined querying language.

## How to run
To run the project, you will need the .NET Framework (the project targets .NET Framework 4.7.2) and an IDE that supports C#.
For example, to run the project in Visual Studio:
1. Open the 'ResearchCollector' solution in Visual Studio
2. Install dependencies (System.Text.Json, System.Device)
3. Press F5 or the 'Start' button.

When running the program, you will be greeted with a window that has three tabs: one for the Filter, one for the Importer and one for the API.

On the Filter tab, you first need to select the type of data set to parse, which can be DBLP, PubMed or PURE. For DBLP, the user must provide the dblp.xml file that can be found here: https://dblp.org/xml/. For PubMed, the program will download and decompress, and parse every one of the 1114 files that the data set consists of automatically, without the user needing to provide any input file. For PURE, the program expects A series of XML files that represent the data that can be extracted from the PURE systems using their api (see https://research-portal.uu.nl/ws/api/523/api-docs/index.html). Additionally, a folder where the output json file should be saved to has to be selected. If no location is selected before pressing the 'Run' button, the program will prompt the user to select one anyways. For convenience in testing, it has been made possible to add a config.txt file to the folder of the 'ResearchCollector' project, where the first line is the path to the input file (only applicable to DBLP and PURE) and the second line is the path to the folder to save the output to.

On the Importer tab, you can click the 'Import Native JSON' button to select a native json file from your local disk and have the importer parse the file and add every publication that is found in the datastructures in memory. If the Filter was run before this without closing the application in between, the output location that was entered in the Filter will automatically be used as the input location for the Importer. Note that the format of the native json file is fixed, and the program only works with the correct native json format (as output by the Filter). Clicking the 'Download Pdf Text' button will instruct the program to attempt to donwload the pdf and extract from that the text for every publication currently stored in memory, after providing an output location to save the .txt files to. The 'Database statistics' table will show some statistics about the data that is currently stored in memory. The 'Import Memory JSON' button will make the program import the data previously exported to a json file (note, not the native json file). The latter can be done with the last button on this tab, 'Export to JSON', which, intuitively, will export the data that is currenlty in memory.

On both the Filter and the Importer tab, there is a checkbox 'Log actions'. When checked, every action performed by the background worker thread will be send to the UI and written to the respective textboxes that serve as the logs on each tab. Actions include: donwloading a file, parsing a publication, finishing the current exection, etc., but not errors, as these are always logged.

On the API tab, you'll find the same table of database statistics as on the Importer tab. A query can be provided in the textbox at the top of the tab. This query must be in the correct format, which is explained with labels in the API tab. To run a query, the user must select the type of items to search for and the method to use for searching. The former can be one of the following: Articles, Inproceedings, Authors, Persons, Journals, Proceedings or Organizations. The method to search for is either Exact or Loose. The former will match only the publications that match **all** predicates provided in the query, whereas Loose searching will match every publication that satisfies at least **one** of the query predicates. Clicking the 'Search' button will, given that the query is in the correct format, find the matching publications in the memory and collect these into a json file, whose output location is given by the user. The number of results will also be displayed in the log on the API tab.

## Future
Many challenges were faced in the making of this project, and lots of compromises and shortcuts had to be taken. Some ideas for features that could be implemented in the future include:
- Adjusting/expanding the Importer to work with an actual database
- Setting up a system that automatically updates the native json files or database instead of having to manually run the Filter
- Finding a better, more consistent way to download pdf's of the publications in memory.
- Implementing ways to extract additional data that was not provided in the data sets from the text of a pdf. For example, the affiliation of authors.
- Perform other data cleaning for the various missing data in the native json files. For example, a lot of authors are missing orcid's or other unique identifiers.

## Credits
This program was written by Thijn Kroon and Matteo Bertorotta under the supervision of Yannis Velegrakis (https://www.uu.nl/medewerkers/IVelegrakis).
