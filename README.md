# ionosphere
[![Build Status](https://dev.azure.com/awuelfing/Ionosphere/_apis/build/status/Build%20and%20push%20Ws?branchName=master)](https://dev.azure.com/awuelfing/Ionosphere/_build/latest?definitionId=1&branchName=master)

Miscellaneous C#/.NET Core code related to Amateur Radio

This repo is mostly infrastructure to do some RBN spot analysis, at least at this point.  Early in this project I looked for a C# (or C-style) implementation of a CTY.DAT lookup algorithm but wasn't able to find one, so I wrote my own (/DXLib/CtyDat/Cty.cs) and made this repo public.  Similarly, this repo has a C# implementation of a Maidenhead Locator-to-lat/long algorithm (/DXLib/Maidenhead/MaidenheadLocator.cs)(and the reverse when I get around to it).  There's also a script to scrape Reverse Beacon Network node locations.  The web services are mostly the infrastructure needed to do the spot analysis.




The source for CTY.DAT is https://www.country-files.com/cty-dat-format/

The source for the Super Check Partial file is https://www.supercheckpartial.com/

The source for callsign information is https://www.hamqth.com/
