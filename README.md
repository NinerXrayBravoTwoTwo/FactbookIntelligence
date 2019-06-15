# Factbook Intelligence

What is the difference between intelligence and data?
-
This project parses "CIA Factbook" (CIAF) data and attempts to turn some of it into "intelligence".

"Data and algorithms are not technology, they are artifacts of technology." -- Quote from myself in 1992

Theory of Intelligence 
-
This project represents the practice of a science.  A STEM of journalism and persuasion.  Most of all it is the art of finding actual truth, incorporating it in yourself, and accepting when your world view is discovered to be in error.  

Boyd and OODA
-
The "Scientific Method (SM)" is the basis of turning data into intelligence, however we have no time for formal review, publishing, peer review, that may take a lifetime.  Instead we MUST use a more intense version of SM; Boydes� OODA loop.

In summary; (O) Take raw data -> (O orient) Huh? what is this -> (D decide) Create Testable hypothesis -> (A) Act/Test -> push feedback into start. (AAR, after action review)

Through this process we can get/approach correct "intelligence".

This is a leadership technique, it is a persuasion technique, 

https://commons.wikimedia.org/wiki/File:OODA.Boyd.svg#/media/File:OODA.Boyd.svg

After discovering OODA, we might think that this has nothing to do with "intelligence" science or understanding.  On the contrary this is essential and the core of ALL intelligence.  Without some variation of this process data might just as well be random drops in an ocean.  To organize and Act on them is what Intelligence is about. 

Factbook Data Source
-
For parseable factbook data I am currently using Ian Coleman's project. As of now there is no API from the DOD that allows direct access to this data.  Ian's project is a python app that scans/mines the CIA-Factbook html pages for data.

https://github.com/iancoleman/cia_world_factbook_api

Factbook Data Quality
-
It is obvious to me that CIAF is a sanitized extraction of basic data that the DOD gathers continuously from around the world.  There is much I would like to know that is missing, but then If I were privy to such information, I couldn't talk about it, so this situation is fine.

This CIAF data set is very small.  It is a 15 MB Json data file which compresses to only 3 MB with gzip when the redundancies are removed (See Entropy (Information Theory on why this is a significant measure).  Therefore, I maintain this is only a pitifully small atom of data the DOD collects.  I'm not sure what their purpose(s) in providing this data is(are).

Testing Data Quality
-
Sources of errors in factbook may be either by commission or omission;
attributable to; Intentional propaganda by DOD or data manipulation by third parties (foreign & domestic),  
Data collection and reporting errors,
Data aggregation errors

Detectable Aggregation Errors
-
Some of the data for a country is expressed as percent of a whole.  If the sum of a set of percentages is not 100 then we can identify a simple aggregation error and work with it.

CIAF has a world summary for all country data.  This can be used to find gross aggregation errors.  There are also sub countries such as "EU" which are composite of their members

Third Party Data Sources
-
Not easy but it is possible to gather firsthand data that is more up to date than CIAF from internet and other pubic data sources.  This can be used to validate or supplement CIAF.

For example; Statistics in Coal production and consumption are not provided in Factbook.  This seems to me a curious oversite because world electric and steel production is very reliant upon coal.  Coal usage statistics are available from third party sources and am considering supplementing CIAF data with these third-party data sources.

Note however that Electricity from Fossil fuels statistic includes electric production from coal.

"Electricity - from fossil fuels Electricity - from fossil fuels field listing 
This entry measures the capacity of plants that generate electricity by burning fossil fuels (such as coal, petroleum products, and natural gas), expressed as a share of the country's total generating capacity."

Purpose/Hypothesis
-

My motive is simple; To understand the economic realities and facts of the "Climate Change" narrative in order to determine their voracity and motivations.

This project represents a starting point for our improvement of perception of the world. We start with a preconceived notion of how things are.  If intelligence is practiced correctly, we will always find aspects of our preconceptions that are in error.  When we learn the facts and their context they will not agree with your friends, the media, your professors, generally accepted theories, or ourselves.  Often reality crystallizes into truths that are so profound it may take years before we accept them let alone are able to convince others. Look at AE's work for example.  It took him years to understand and accept some of the implications of his early work.  He never did fully believe the implications of quantum mechanics.  And today, 100 years later, most of the human race still does not even partially believe/understand it. 

Always consider the source and motives of intelligence. For example, "Journalism" usually starts from the story the editor wants to tell and works backwards to find "evidence/facts" that support that narrative.  If the journalist and editor have integrity, they will change their story to reflect facts and not ignore facts that don't support their story. This is a reason ACTUAL intelligence is often a secret, in order to figure out what is going on away from political influences. 

Method
-
Every search starts someplace; I have started with physics and general relativity. This perspective allows me to understand the energies expended in the earths/sun/space climate system and comprehend these energies.  Note; Nagasaki bomb released 1g of energy (see note at end of this file for more or reference my Mass-Energy GitHub project.)

To keep everything in context I occasionally pop out to the 10 AU view (~1.4 hours distance from sun, twice as far as Jupiter�s orbit.)  From here it is easier to remember that, we and everything are in space, that the earth is very close to our star, which is incredibly hot and bright, surrounded by nothing at all for light years in every direction. Our arguments about climate change are insignificant relative to the reality in which we actually exist.

Procedures
-

Analysis
-
Brief Relativity Summary
-
Energy is very difficult for people to comprehend. We understand things we can hold, lift, and see.  If you knew that the USA consumed 3,902 tera watt hours in 2016 it seems incomprehensible.  What if I told you that this could be loaded into the back of your truck and would weigh 344.6 lbs.? (156.3 kg)

Time is not a special unit; it is part of space and should be measured in meters, specifically three hundred million meters. Substituting the new value for s makes the speed of light a unitless constant equal to one.

When you substitute s=300 Mm into Newtonian physics, energy and mass become the same thing. A kg is 89.9 Peta J or 25 Tera watt hrs.  This might take 186k miles to absorb.

Note: In AE�s e=mc^2, c^2 is just a conversion factor, ergs to grams. The speed of light, c=1 after all.

Our star radiates 385 yotta watts about 0.1 x the mass of Mt Everest per hour or 15,000 trillion Nagasaki�s per hr.
(yotta = trillion trillion)

How much energy the earth absorbs and radiates per day seems to be a closely guarded secret.  Clearly, absorption and radiation are in equilibrium because the earth is not getting warmer or cooler within any margin of error we can currently measure.

Note: Mass is not matter; it is an attribute of matter. 

Note: the Nagasaki bomb released 1g of energy, a 21 mega ton bomb releases a kg of energy, 1000 x Nagasaki.  Annual world electricity production is about 24,000 TWh or 947 kg (947,000 Nagasaki sized bombs. I am aware of the rounding error here but am trying to communicate not get lost in the weeds. One Nagasaki is actually 1.0242 g)

AARS
-
**June 15, 2019;** Also mising from the data are Uranium / Thorium mining for reactor fuel.  Not significant today but in 100 years?

*8 kWh of heat can be generated from 1 kg of coal, approx. 12 kWh from 1 kg of mineral oil and around 24,000,000 kWh from 1 kg of uranium-235. Related to one kilogram, uranium-235 contains two to three million times the energy equivalent of oil or coal. The illustration shows how much coal, oil or natural uranium is required for a certain quantity of electricity. Thus, 1 kg natural uranium - following a corresponding enrichment and used for power generation in light water reactors - corresponds to nearly 10,000 kg of mineral oil or 14,000 kg of coal and enables the generation of 45,000 kWh of electricity.*

**June 15, 2019;** Relating data in energy sectors to Econ growth.
```
ekg     eFF_kg  Fuel_Mbbl       NatGas_Gcm      Co2Tton $Growth 
```
Switch back to kg of energy from Tera watt's because it is eaiser to compare/understand and for ordinary humans to relate too.  Add in fuel and natGas to table.
Problems;  There is no value for Coal consumption.  It is mixed into FF electric BUT there is no separate statistic for other uses of coal such as direct coal steal making industry, paper, heading, steam production, steam powered trains (china/VN) etc.  Not all coal is consumed in electric generation.

**June 14, 2019;** Reduced all the data concepts into a single story/question; �How is country economics related to CO2 emissions.  Many ways to go with this; Am currently testing three views, 1> FF Electric Production, 2> CO2 Emissions, 3> Economic growth.

Some problems; a> Only the top 40 or so countries by total Power production have an economy b> Some countries are in growth decline or collapse due to war (Venezuela, Syria, Saudi Arabia, Iran, Iraq)

**June 11, 2019;** Processed a series of pie charts detailing crude oil, refined oil, electric production, and CO2 emissions.  Converted some of this data into color coded world maps. Interesting results.  Each graph poses more experiments and questions. Am considering changes to output graphs directly instead of post processing the data in Excel.

Published the results on twitter and did not receive much feedback. I�m wonder if it needs to be packaged better. Where else could I publish this intelligence?  Who needs this intelligence?


