# CodeSpider
The following is part of the a blog post I made about this software. The post can be found on https://isaacwyatt.com.

# Summary / TLDR
CodeSpider was a utility I wrote in C# in 2008/2009 as a means to evaluate the html of a large number of web pages to identify which pages contained certain IDs that needed to be changed or removed. It was the first 'useful' software I'd written.

# Background
In 2008 / 2009 time frame, Websites for B2B Software Companies, were often managed via cumbersome content management systems, and integration with newer tools like Salesforce and Eloqua (before Marketo came on the scene), often required making many manual changes, implementing static data, etc. As an example, a user filling out a form on a website requesting a free trial, on submitting the form would pass along a hidden `SFID` variable to be processed by the server receiving the form submission. The value in this variable would attach a person's name (A "Lead") to a particular Salesforce.com Campaign object. Often, these SFIDs would get phased out, updated, and replaced, but tracking down which web pages contained IDs that needed replaced was often a whack-a-mole process that was incomplete and time-consuming, and would screw up reporting.

>[!note] It might be that at this time, the "Content Management System" was my employer's own, home-grown, web server. So I imagine that this whole exercise could have been made unnecessary by a simple bash script. 

# Problem
Among hundreds (thousands?) of pages that would have these ids, a subset would always need updated, but determing which ones was, at best, an educated guess based on (1) a loosely compiled history of which pages were created in the previous time period and (2) seeing which Salesforce.com Leads were recently attached to old Campaigns. This resulted in many hours of manually checking all the likely web pages for the code that needed to be retired, but was inevitably incomplete. Additionally, this resulted in business reports being inaccurate such that driving decision making was sub optimal. 

>[!note] When a Lead was attached to an old campaign that it was not supposed to, people would come by my desk in a slightly accusative tone: `Why are my leads getting attached to old campaigns?`. It was a valid question, and as a young person recently out of college in their first professional job, I probably took it too personally. I got tired of these conversations and explaining what the problem was, and that our in-house web team weren't really invested in helping me solve the problem. I wasn't going to spend weeks going through potentially thousands of web pages trying to find every instance every time we retired a campaign, so my motivation to dust off my coding experience and learn my first language since Qbasic to solve a real, practical problem.

>[!note] No one has ever hired me to write code or develop intellectual property. My job titles have nearly all been of the "Strategy and Operations" variety which is usually strictly execution of other internal team's priorities. No job description I've ever signed an offer for included any requirements to develop code. At the time I'd written this software, I'd never worked for a company that required an intellectual property ownership agreement of me. This software clearly had no marketable value as not a single person ever requested to download it and use it themselves.

# Solution
To provide a comprehensive set of pages for which the IDs would need to be changed, I wrote a command line tool that would crawl the set of pages specified in a text file and return the list of web page urls that contained a user-specified search phrase, and what line the searched phrase was on.

This list of URLs would then be provided to the web development team to update the pages in question.

# Impact
By automating the process of opening a web page, searching the source code for a particular match, and creating a list of urls that needed updating, I saved myself hundreds of hours of labor, improved accuracy of reporting, and potentially helped assure that bonuses for performance were paid. Not to mention some leads that might have been ignored otherwise, were followed up on by sales staff. I also learned coding skills to unlock future leverage and efficiencies within the company I worked for at the time.

# Known Limitations
CodeSpider only searches the source code, and not the rendered HTML of the page it fetched. It probably would not work behind a proxy or firewall. It is unknown if it would even compile and run on modern systems. CodeSpider is not maintained or supported.

# Alternative Solutions
I imagine that there were many other solutions to solving this problem, including but not limited to, using ever-green Campaigns in Salesforce, or using Lead-Campaign association logic in the SFDC or Marketing Automation layer, or even writing a server-side bash script to regex-replace to perform the necessary updates. Today, the whole architecture that modern websites use, the problem _should be_ moot.

# Further Development
If this had been an actually useful tool, I think further development would have been to add a Graphic User Interface, and to search both the source code and the rendered web page for particular search strings. It could have also been updated to operate like a web crawler, following all outbound links to search a broader network of pages for potentially required updates.

# Conclusion
Searching the conetnts of hundreds, maybe thousands, of webpages can be automated easily in order to identify the pages that may need updating. Without knowing it, I added hundreds of thousands of dollars of value to the company.