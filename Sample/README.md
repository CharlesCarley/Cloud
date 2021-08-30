# BookStore

It a test storage API. The purpose of this sample is to provide a working environment to test database access. 

It's split into separate component projects.

- [BookStore](#bookstore)
  - [BookStore.Api](#bookstoreapi)
  - [BookStore.Cli](#bookstorecli)
  - [BookStore.Client](#bookstoreclient)
  - [BookStore.Store](#bookstorestore)
  - [BookStore.FrontEnd](#bookstorefrontend)
  - [BookStore.Mobile](#bookstoremobile)

## BookStore.Api

Defines the API that gets feed into both the client and server projects.

## BookStore.Cli

Contains a basic command line tool to access database store service.   

## BookStore.Client

Contains the local database code that is generated from the BookStore.Api assembly.

## BookStore.Store

Contains the external database code that is generated from the BookStore.Api assembly.

## BookStore.FrontEnd

Contains a basic CRUD application to modify the database store service.   

## BookStore.Mobile

Not implemented at the moment, but it should be setup to cache the server data locally and
work from the cache. 
