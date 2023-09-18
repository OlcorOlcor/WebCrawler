# WebCrawler
This app was made as seminar project for NSWI153: Advanced Technologies for Web Applications in summer semester 2022/23. Technologies we decided to use are mainly ASP.NET for backend and Svelte for frontend. Furthermore application uses GraphQL and crawled data can be queried using GraphQL.

## About project
The main idea is to make a server, that periodicaly goes through websites and checks for links to other pages. First user add Record by entering page (URL) that the process should start on and periodicity of execution. User also enters a boundary regex to specify which pages are they interested in and pages not complying to that regex are not further crawled. The whole process is saved and displayed on the bottom of the page via graph. Graph can be switched to domain view where all pages of the same domain are compressed to one node and can be make static, so that it is not automaticaly updated as crawling continues. Other main features on page are list of all Records, where user can start another execution manualy and also delete the whole record, and list of Executions, where last finished and all running executions of records are shown. Further info is diplayed in both tables.

## Deploynment
The whole app can be deployed simply by cloning the repository and running a docker container like this:
```console
git clone ... (cloning link)
docker compose up --build
```