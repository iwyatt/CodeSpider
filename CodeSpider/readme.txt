Codespider V.0.0.5 5/18/2009

To install and use CodeSpider, unzip the codespider.exe file into the desired directory ("c:\codespider" is recommended).
When ready, Click on your Start menu, and select Run, or hold the Windows key on your keyboard and press 'R'.
Type 'CMD' in the Run menu, and press enter.

Navigate to the directory where you unzipped the codespider.exe file by typing in "cd\[yourdirectory]" and press enter.

Now type in "codespider" for a list of command line options to search your web pages:

Syntax:
Codespider /i:[csvfile] /w:[website to search] /s:[searchstring] /r:[regularexpression] /o:[outputfile]

Arguments:
/i:      the path and location of a csv containing urls of pages to search. Each entry should be on its own line.
/w:      a single web address to search. Can also be used in conjunction with /i: .
/s:      the search term that you want CodeSpider to find. Use \ to escape double quotes.
/r:      a regular expression search (advanced user option).
/o:      the file location for text output in csv.

Example Input:
Codespider /i:c:\mycsv.csv /o:results.csv /w:http://www.yahoo.com/index.html /s:"<P ALIGN>"

Example Output:
https://isaacwyatt.com             Matched phrase "<P ALIGN>" at line 14
http://www.yahoo.com/index.html    No match found for phrase "<P ALIGN>" -1