## Roadmap
This is more of a backlog than roadmap, these stories are in no particular order.

###Refactors
1. [DONE] Remove RavenDb and put MongoDB there
2. Ensure all repository actions go through a service.
3. Re-engineer the runner (pair up)
4. Better test coverage of the runners, docs.

###Documentation and setup
1. Cleanup the PS scripts
2. A better MongoDB setup experience
3. API documentation
4. XML documentation

###Editor UI
1. UI: Add new test cases.
2. UI: Delete existing ones.

###Conversion
1. Command line tool to convert old XML cases to new.
2. Plugin architecture for the tool to modify the XML during various stages of the conversion.
3. Bespoke plugins for the converter console tool.

###Results
1. Save HTML output from results.
2. View HTML results as part of the main result (raw and rendered).
3. Better UI for the results list: group by team, display date, time of run etc.
4. Better UI for a single test run result: tables instead of cards, links to HTML output of each result.

###Running tasks
1. Page to show running tasks.
2. Ability to stop a running task.
3. Audit trail for who started and stopped a task.

###Security
1. Add authentication provider (using LDAP) and login page.
2. Add temporary admin login details to the web.config.

###Teamcity
1. Teamcity plugin that rrunnerses via the Syringe REST api.
