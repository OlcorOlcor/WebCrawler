# WebCrawler
This app was made as seminar project for NSWI153: Advanced Technologies for Web Applications in summer semester 2022/23. Technologies we decided to use are mainly ASP.NET for backend and javascript framework Svelte for frontend. Furthermore the application uses the GraphQL.AspNet library and crawled data can be queried using GraphQL.

## About project
The main idea is to make a server, that periodicaly goes through websites and checks for links to other pages. First user adds a record by entering page (URL) that the crawling process should start on and periodicity of the execution. User also enters a boundary regular expression to specify which pages are they interested in and pages not complying to that regular expression are not further crawled. The whole process is saved and displayed on the bottom of the page via a graph. The graph can be switched to a domain view where all pages of the same domain are compressed into a one node. The graph can also be made static, so that it is not automatically updated as crawling continues. Other main features on the page are a list of all records, where user can start another execution manually and also delete the whole record, and list of Executions, where the last finished and all running executions of records are shown. Further info is diplayed in both tables.

## Deployment
The whole app can be deployed simply by cloning the repository and running a docker container like this:
```console
git clone ... (cloning link)
docker compose up --build
```
