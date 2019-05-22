# Factbook Intelligence

What is the difference between intelligence and data?  This project parses "CIA Factbook" (CIAF) data and attempts to turn some of it into "intelligence".

Theory Of Inteligence 
-
This is the practice of science.  A STEM of journalism and persuation.  Most of all it is the art of finding the truth and incorporating it in yourself and accepting when your world view is discovered to be in error.  

Boyd and OODA
-
Formal science is old and dusty.  The "Scientific Method (SM)" is essential however we have no time for formal review, publishing, pier review, that may take a lifetime.  Instead we MUST use a different method that looks very similar to the SM; Boyde's OODA loop.

In summary; (O) Take raw data -> (O) ohhh crud -> (O orient) Huh? what is this -> (D decide) Create Testable hypothesis -> (A) Act/Test -> push feedback into start. (AAR, after action review)

Through this process you can get/approch correct "intellegence".

This is a leadership technique, it is a persuation technique, 

https://commons.wikimedia.org/wiki/File:OODA.Boyd.svg#/media/File:OODA.Boyd.svg

After discovering OODA you might think that this has nothing to do with "intelligence" science, or undrestanding.  On the contrary this is essential and the core of ALL intelligence.  Without some variation of this process data might just as well be random drops in an ocean.  To organize and Act on them is what Intelligence is about. 

Faster ...

Itteration
-
For parsable factbook data I am currently using Ian Coleman's project. As of now there is no API from the DOD that allows direct access to this data.  Ian's project is a python app that scan's mines the CIA-Factbook pages for data.

https://github.com/iancoleman/cia_world_factbook_api

CIA Factbook Data Quality
 
It is obvious to me that CIAF is a sanatized extraction of basic data that the DOD gathers continously from around the world.  There is much I would like to know that is missing, but then If I were privey to such information I couldn't talk about it so this is situation is fine.

The CIAF data set is very small.  It is a 15 MB Json data file which compresses to only 3 MB with gzip when the redundencies are removed (See Enthropy (Information Theory on why this is a significant measure).  This is why I maintain this is only a pitifully small atom of data the DOD collects.  I'm not sure what their purpose(s) in providing this data is(are).

Testing Data Quality
-
Sources of errors in factbook may be  either by commission or omission;
attributeable to; Intentional propaganda by DOD or data manipulation by third parties (foriegn & domesic),  
Data collection and reporting errors,
Data aggregation errors

Detectable Aggregation Errors

Some of the data for a country is expressed as percent's of a whole.  If the sum of a set of percents is not 100 then we can identify a simple aggration error and work with it.

CIAF has a world summary for all country data.  This can be used to find gross aggrigation errors.  There are also sub countries such as "EU" which are composits of their members

Third Party Data Sources

Not easy but it is possible to gather first hand data that is more up to date than CIAF from internet and other pubic data sources.  This can be used to validate or suplement CIAF.

Purpose/Hypothesis
-
Always consider the source and motives of intelligence. For example "Journalism" usually starts from the story the editor wants to tell and works backwards to find "evidence/facts" that support the narative they want to tell.  My motive is simple; To understand the economic realities of the "Climage Change" narative with actual facts and determine the voracity of Climate change motivations.

Every search starts someplace, Orientaiton, for this I take my decdes of software development experiance

Procedures
-

Analysis
-

Feedback
-
