# BookStore

Is a test storage API. Its purpose is to provide a working environment to test database access. 

It's split into separate component projects.

- [BookStore](#bookstore)
  - [BookStore.Api](#bookstoreapi)
  - [BookStore.Cli](#bookstorecli)
  - [BookStore.Client](#bookstoreclient)
  - [BookStore.Store](#bookstorestore)
  - [BookStore.FrontEnd](#bookstorefrontend)
  - [BookStore.Mobile](#bookstoremobile)

## [BookStore.Api](BookStore/BookStore.Api)

Defines the development API that gets feed into both the client and server projects.

## [BookStore.Cli](BookStore/BookStore.Cli)

Contains a basic command line tool to access the database service. 

## [BookStore.Client](BookStore/BookStore.Client)

Contains the local database code that is generated from the BookStore.Api assembly.

## [BookStore.Store](BookStore/BookStore.Store)

Contains the external database code that is generated from the BookStore.Api assembly.

## [BookStore.FrontEnd](BookStore/BookStore.FrontEnd)

Contains a basic web application to modify the database service.    

## BookStore.Mobile

Not implemented at the moment, but it should be setup to cache the server data locally and
work from the cache. 
