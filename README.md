# Factbook Intelligence

## Intelligence Or/Vs Data?

This project parses "CIA Factbook" (CIAF) data and attempts to turn some of it into "intelligence".

"Data and algorithms are not technology, they are artifacts of technology." -- Quote from myself in 1992


## Purpose/Hypothesis


My motive is simple; To understand the economic realities and facts of the "Climate Change" narrative in order to determine their voracity and motivations.

This project represents a starting point for our improvement of perception of the world. We start with a preconceived notion of how things are.  If intelligence is practiced correctly, we will always find aspects of our preconceptions that are in error.  When we learn the facts and their context they will not agree with your friends, the media, your professors, generally accepted theories, or ourselves.  Often reality crystallizes into truths that are so profound it may take years before we accept them let alone are able to convince others. Look at AE's work for example.  It took him years to understand and accept some of the implications of his early work.  He never did fully believe the implications of quantum mechanics.  And today, 100 years later, most of the human race still does not even partially believe/understand it. 

Always consider the source and motives of intelligence. For example, "Journalism" usually starts from the story the editor wants to tell and works backwards to find "evidence/facts" that support that narrative.  If the journalist and editor have integrity, they will change their story to reflect facts and not ignore facts that don't support their story. This is a reason ACTUAL intelligence is often a secret, in order to figure out what is going on away from political influences. 

## Method

Every search starts someplace; I have started with physics and general relativity. This perspective allows me to understand the energies expended in the earths/sun/space climate system and comprehend these energies.  Note; Nagasaki bomb released 1g of energy (see note at end of this file for more or reference my Mass-Energy GitHub project.)

To keep everything in context I occasionally pop out to the 10 AU view (~1.4 hours distance from sun, twice as far as Jupiter’s orbit.)  From here it is easier to remember that, we and everything are in space, that the earth is very close to our star, which is incredibly hot and bright, surrounded by nothing at all for light years in every direction. Our arguments about climate change are insignificant relative to the reality in which we actually exist.

#### Theory of Intelligence 

This project represents the practice of a science.  A STEM of journalism and persuasion.  Most of all it is the art of finding actual truth, incorporating it in yourself, and accepting when your world view is discovered to be in error.  

#### Boyd and OODA

The "Scientific Method (SM)" is the basis of turning data into intelligence, however we have no time for formal review, publishing, peer review, that may take a lifetime.  Instead we MUST use a more intense version of SM; Boydes’ OODA loop.

In summary; (O) Take raw data -> (O orient) Huh? what is this -> (D decide) Create Testable hypothesis -> (A) Act/Test -> push feedback into start. (AAR, after action review)

Through this process we can get/approach correct "intelligence".

This is a leadership technique, it is a persuasion technique, 

https://commons.wikimedia.org/wiki/File:OODA.Boyd.svg#/media/File:OODA.Boyd.svg

After discovering OODA, we might think that this has nothing to do with "intelligence" science or understanding.  On the contrary this is essential and the core of ALL intelligence.  Without some variation of this process data might just as well be random drops in an ocean.  To organize and Act on them is what Intelligence is about. 

#### Factbook Data Source

For parseable factbook data I am currently using Ian Coleman's project. As of now there is no API from the DOD that allows direct access to this data.  Ian's project is a python app that scans/mines the CIA-Factbook html pages for data.

https://github.com/iancoleman/cia_world_factbook_api

##### Factbook Data Quality

It is obvious to me that CIAF is a sanitized extraction of basic data that the DOD gathers continuously from around the world.  There is much I would like to know that is missing, but then If I were privy to such information, I couldn't talk about it, so this situation is fine.

This CIAF data set is very small.  It is a 15 MB Json data file which compresses to only 3 MB with gzip when the redundancies are removed (See Entropy (Information Theory on why this is a significant measure).  Therefore, I maintain this is only a pitifully small atom of data the DOD collects.  I'm not sure what their purpose(s) in providing this data is(are).

##### Testing Data Quality

Sources of errors in factbook may be either by commission or omission;
attributable to; Intentional propaganda by DOD or data manipulation by third parties (foreign & domestic),  
Data collection and reporting errors,
Data aggregation errors

##### Detectable Aggregation Errors

Some of the data for a country is expressed as percent of a whole.  If the sum of a set of percentages is not 100 then we can identify a simple aggregation error and work with it.

CIAF has a world summary for all country data.  This can be used to find gross aggregation errors.  There are also sub countries such as "EU" which are composite of their members

#### Third Party Data Sources

Not easy but it is possible to gather firsthand data that is more up to date than CIAF from internet and other pubic data sources.  This can be used to validate or supplement CIAF.

For example; Statistics in Coal production and consumption are not provided in Factbook.  This seems to me a curious oversite because world electric and steel production is very reliant upon coal.  Coal usage statistics are available from third party sources and am considering supplementing CIAF data with these third-party data sources.

Note however that Electricity from Fossil fuels statistic includes electric production from coal.

"Electricity - from fossil fuels Electricity - from fossil fuels field listing 
This entry measures the capacity of plants that generate electricity by burning fossil fuels (such as coal, petroleum products, and natural gas), expressed as a share of the country's total generating capacity."

## Procedures


## Analysis Summary

Raw analysis is collected in the After-Action Review (AAR) list at the end of this file.

### Summary of World Energy Findings

#### Fossil Fuel burning to generate Electric - Highest correlation
ff: N: 14 Mean: 30.24 Slp: 150.12 Cor: 0.9878 Qx: 12.272 Qy: 1865.048 Y: 1682

#### Electric Production - High Correlation
elecprod: N: 14 Mean: 48.68 Slp: 98.86 Cor: 0.9842 Qx: 18.567 Qy: 1865.048 Y: 1409

#### Electric consumption - High correlation
eleccons: N: 14 Mean: 45.11 Slp: 103.76 Cor: 0.9816 Qx: 17.645 Qy: 1865.048 Y: 1541

#### Refined Fuel consumption - Good correlation
fuel: N: 14 Mean: 4.40 Slp: 1152.18 Cor: 0.9113 Qx: 1.475 Qy: 1865.048 Y: 1153

#### Natural Gas consumption - Correlation Not so good
natgas: N: 14 Mean: 157.36 Slp: 19.96 Cor: 0.6239 Qx: 58.290 Qy: 1865.048 Y: 3081

#### CO2 Emissions - Good correlation
emission: N: 14 Mean: 1931.45 Slp: 2.05 Cor: 0.9423 Qx: 859.299 Qy: 1865.048 Y: 2272

#### GDP to GDP Growth rate - Very low correlation
growth: N: 14 Mean: 3.26 Slp: 1072.37 Cor: 0.3594 Qx: 0.625 Qy: 1865.048 Y: 2729
There is no clear relation between a countries current GDP and that countries rate of growth.  i.e. This disproves the general assertion that "The rich get richer and the poor get poorer.

#### Growth to non-average energy production

The experiment to compare countries growth rate to countries non average energy production ... showed no correlation.  i.e. if a country generates more or less energy per GDP than average it does not noticeable effect that countries growth rate.

## Brief Relativity Summary

Energy is very difficult for people to comprehend. We understand things we can hold, lift, and see.  If you knew that the USA consumed 3,902 tera watt hours in 2016 it seems incomprehensible.  What if I told you that this could be loaded into the back of your truck and would weigh 344.6 lbs.? (156.3 kg)

Time is not a special unit; it is part of space and should be measured in meters, specifically three hundred million meters. Substituting the new value for s makes the speed of light a unitless constant equal to one.

When you substitute s=300 Mm into Newtonian physics, energy and mass become the same thing. A kg is 89.9 Peta J or 25 Tera watt hrs.  This might take 186k miles to absorb.

Note: In AE’s e=mc^2, c^2 is just a conversion factor, ergs to grams. The speed of light, c=1 after all.

Our star radiates 385 yotta watts about 0.1 x the mass of Mt Everest per hour or 15,000 trillion Nagasaki’s per hr.
(yotta = trillion trillion)

How much energy the earth absorbs and radiates per day seems to be a closely guarded secret.  Clearly, absorption and radiation are in equilibrium because the earth is not getting warmer or cooler within any margin of error we can currently measure.

Note: Mass is not matter; it is an attribute of matter. 

Note: the Nagasaki bomb released 1g of energy, a 21 mega ton bomb releases a kg of energy, 1000 x Nagasaki.  Annual world electricity production is about 24,000 TWh or 947 kg (947,000 Nagasaki sized bombs. I am aware of the rounding error here but am trying to communicate not get lost in the weeds. One Nagasaki is actually 1.0242 g)

# AARS

## June 19, 2019

The test to compute linear regressions using standard deviations of other variables showed that no such relation can be found. It's equivalent to taking a second integration of the data and other dimensions would have to be used to find a correlation.  We simply do not have the data for that complicated a model.  

Software Design; The new structure of the classes is better, but I can delete the now extraneous code for the new standard deviation vs Growth computations.  

Next; improve the report format as a pdf and look at integrating PD map models and graphics so I don’t need to import the data to excel for graphics.

-----

The linear regressions (LR) I calculated for CIAF (CIA Factbook) data have the following values,

 Each LR is bivariate, X / Y where X is the item in question and Y is the GDP - Purchase Power Parity for each country
```
MergePowerData.Report. StatCollector

Electric Production - High Correlation
elecprod: N: 14 Mean: 48.68 Slp: 98.86 Cor: 0.9842 Qx: 18.567 Qy: 1865.048 Y: 1409.83768051239

Electric consumption - High correlation
eleccons: N: 14 Mean: 45.11 Slp: 103.76 Cor: 0.9816 Qx: 17.645 Qy: 1865.048 Y: 1541.89389252972

Refined Fuel consumption - Good correlation
fuel: N: 14 Mean: 4.40 Slp: 1152.18 Cor: 0.9113 Qx: 1.475 Qy: 1865.048 Y: 1153.10754020737

Natural Gas consumption - Correlation Not so good
natgas: N: 14 Mean: 157.36 Slp: 19.96 Cor: 0.6239 Qx: 58.290 Qy: 1865.048 Y: 3081.13951583579

Fossil Fuel burning to generate Electric - Highest correlation
ff: N: 14 Mean: 30.24 Slp: 150.12 Cor: 0.9878 Qx: 12.272 Qy: 1865.048 Y: 1682.88703120591

CO2 Emissions - Good correlation
emission: N: 14 Mean: 1931.45 Slp: 2.05 Cor: 0.9423 Qx: 859.299 Qy: 1865.048 Y: 2272.11573250234

GDP to GDP Growth rate - Very low correlation
growth: N: 14 Mean: 3.26 Slp: 1072.37 Cor: 0.3594 Qx: 0.625 Qy: 1865.048 Y: 2729.416922744
```

## June 18, 2019

b> Computed linear regressions for;
	*1> electric prod deviation / gdp.growth; 
	*2> ff consumption deviation / gdp.growth

The results show no correlation at this level of data resolution.  Clearly the rate of electric growth is related to growth of certain industries but not for entire countries where there are other financial factors.

```
N: 14 Mean: -0.77 Slp: -0.60 Cor: -0.1739 Qx: 0.180 Qy: 0.625 Y: 2.79266010063874
N: 14 Mean: -0.91 Slp: -0.65 Cor: -0.1649 Qx: 0.158 Qy: 0.625 Y: 2.66011841105497
```
a> It is unequivocal that country energy usage correlates directly to country GDP.  The remaining question is; Does greater than average energy use correlate to greater economic growth?  I can answer that question or determine if there is not enough fine-grained data in CIAF.

## June 17, 2019

kg|dev_e|eFFkg|dev_FF|FuelMbl|NatGasGcm|Co2Tton|$G PP|Country
---:|---:|---:|-----:|------:|--------:|------:|----:|------
947.3|28.154|596.8|31.231|96.26|3477.0|33620|127800|World
235.6|0.739|146.1|0.366|12.47|238.6|11670|23210|China
164.0|2.494|114.8|1.414|19.96|767.6|5242|19490|United States
121.9|7.598|53.6|11.248|12.89|428.8|3475|20850|European Union
55.5|3.440|39.4|2.994|4.52|55.4|2383|9474|India
41.3|0.176|28.1|0.331|3.65|467.5|1847|4016|Russia
39.6|1.265|28.1|0.948|3.89|127.2|1268|5443|Japan
26.0|0.793|6.0|0.757|2.45|124.4|641|1774|Canada
24.5|1.530|10.1|2.369|2.46|93.4|848|4199|Germany
22.7|0.838|3.9|2.384|2.96|34.4|514|3248|Brazil


b> Refactor statistics to provide standard deviation of value in table for electric Prod / $PP and burning FF / $PP

a> Need to report if a report item is outside standard deviation range for that variable.  Will and insight and understanding/intelligence to the report.

e.g. if the value for world-ekg is 947.3 is that within expected SD range?

## June 16, 2019

Add my statistic class to project. Compute liner regressions on several different numbers relative to GDP. Yes, energy use is clearly related to GDP with a high correlation.

```
ElecC: N: 14 Mean: 48.68 Slp: 98.86  Cor: 0.9842 Qx: 18.567 Qy: 1865.048
BrnFF: N: 14 Mean: 30.24 Slp: 150.12 Cor: 0.9878 Qx: 12.272 Qy: 1865.048
Fuel:  N: 14 Mean: 4.40 Slp: 1152.18 Cor: 0.9113 Qx: 1.475 Qy: 1865.048
NatGas:N: 14 Mean: 157.36 Slp: 19.96 Cor: 0.6239 Qx: 58.290 Qy: 1865.048
CO2:   N: 14 Mean: 1931.45 Slp: 2.05 Cor: 0.9423 Qx: 859.299 Qy: 1865.048
```

## June 15, 2019
Calculate kg of Uranium and exchange $growth for Purchasing Power which is better correlated to CO2 emissions.

ekg|eFFkg|U235kg|FuelMbl|NatGasGcm|Co2Tton|$PP|Country
---:|---:|---:|---:|---:|---:|---:|---
947.3|596.8|94600.0|96.26|3477.0|33620|127800|World
235.6|146.1|7844.0|12.47|238.6|11670|23210|China
164.0|114.8|24570.0|19.96|767.6|5242|19490|United States
121.9|53.6|24344.0|12.89|428.8|3475|20850|European Union
55.5|39.4|1848.0|4.52|55.4|2383|9474|India
41.3|28.1|7560.7|3.65|467.5|1847|4016|Russia
39.6|28.1|659.5|3.89|127.2|1268|5443|Japan
26.0|6.0|3897.6|2.45|124.4|641|1774|Canada
24.5|10.1|2042.7|2.46|93.4|848|4199|Germany
22.7|3.9|378.6|2.96|34.4|514|3248|Brazil
21.2|3.6|17636.7|1.71|41.9|341|2856|France
21.1|14.7|7364.0|2.58|45.3|778|2035|Korea, South
13.0|13.0|0.0|3.29|109.3|657|1775|Saudi Arabia
12.7|6.4|1909.2|1.58|79.2|424|2925|United Kingdom


## June 15, 2019

It should be possible to calculate the U235 equivalent burned in generating electric power.  Equation (%nuclear * ElectricProd / kgU235tokWh).  The conversion factor is 24 mega kWh per kg, which should provide an estimate for Uranium used to compare (Excluding military applications). 

## June 15, 2019
Beside Coal also missing from the data are Uranium / Thorium mining for reactor fuel.  Significant today and more important every year.

*8 kWh of heat can be generated from 1 kg of coal, approx. 12 kWh from 1 kg of mineral oil and around 24,000,000 kWh from 1 kg of uranium-235. Related to one kilogram, uranium-235 contains two to three million times the energy equivalent of oil or coal. The illustration shows how much coal, oil or natural uranium is required for a certain quantity of electricity. Thus, 1 kg natural uranium - following a corresponding enrichment and used for power generation in light water reactors - corresponds to nearly 10,000 kg of mineral oil or 14,000 kg of coal and enables the generation of 45,000 kWh of electricity. *

## June 15, 2019

Relating data in energy sectors to Econ growth.
```
$ ./MergePowerData
```
ekg|eFF_kg|Fuel_Mbbl|NatGas_Gcm|Co2Tton|$Growth|Country
---:|------:|---------:|----------:|------:|------:|-------
947.3|596.8|96.26|3477.0|33620|3.7|World
235.6|146.1|12.47|238.6|11670|6.9|China
164.0|114.8|19.96|767.6|5242|2.2|United States
121.9|53.6|12.89|428.8|3475|2.3|European Union
55.5|39.4|4.52|55.4|2383|6.7|India

Switch back to kg of energy from Tera watt's because it is easier to compare/understand and for ordinary humans to relate too.  Add in fuel and natGas to table.
Problems; There is no value for Coal consumption.  It is mixed into FF electric BUT there is no separate statistic for other uses of coal such as direct coal steal making industry, paper, heading, steam production, steam powered trains (china/VN) etc.  Not all coal is consumed in electric generation.

## June 14, 2019

Reduced all the data concepts into a single story/question; “How is country economics related to CO2 emissions.  Many ways to go with this; Am currently testing three views, 1> FF Electric Production, 2> CO2 Emissions, 3> Economic growth.

Some problems; a> Only the top 40 or so countries by total Power production have an economy b> Some countries are in growth decline or collapse due to war (Venezuela, Syria, Saudi Arabia, Iran, Iraq)

## June 11, 2019

Processed a series of pie charts detailing crude oil, refined oil, electric production, and CO2 emissions.  Converted some of this data into color coded world maps. Interesting results.  Each graph poses more experiments and questions. Am considering changes to output graphs directly instead of post processing the data in Excel.


