MediaWorks Hololens Project Design Document

Overview

Intro

The idea of this project is to build a prototype system that will provide an AR tour experience for the MediaWorks Lab. This will be achieved through an integration of visual cues and gesture commands.. The goal is to be able to scan QR codes situated around the lab that will bring up related content to nearby objects and let the user choose to interact with a single point of interest, or to continue exploring around the lab. By interacting with a point of interest, the user will learn more about that specific object and how it works. The end goal is to create a useful and scalable tool to be used in any museum or showcasing setting.  

The project consists of two major parts:
1. Unity Application for Hololens
2. Database Management System

HoloLens

A user that has a HoloLens on will be able to walk around a room and find QR codes. These QR codes will relate to an object of interest. The QR code can be scanned by a user through taking a picture using the hololens camera as shown below.

![github-large](https://github.com/Jen-Hobbs/HoloLensTourSystem/blob/master/Images/tap%20qr%20code.png?raw=true)

 Once a QR code is recognized by the Hololens an interface will pop up allowing the user to see information about the object of interest. 

![github-medium](https://github.com/Jen-Hobbs/HoloLensTourSystem/blob/master/Images/QR%20code%20recognized.png?raw=true)

This interface shows a picture and text about the object. There are multiple pages available for the user to see and they can move through the pages by saying voice commands or interacting with the buttons. The buttons available allow the user to go to next page or previous page. On the left is a table of contents that allows the user to jump to a specific page. The document can be closed at any time and the user can then move onto a new object of interest.

Website

The website allows an admin to add objects of interest into the database. The Admin is able to create new objects of interest which will result in a QR code being generated. The Admin will be asked to add and image and text information for each page for an object. An object can have multiple pages. The Admin can see all object of interest reprint the QR code, Edit an object, or Delete an Object.

You can find the website portion of this project here https://github.com/joannakwh/HololensWebApp

Technical Specification

For more information about this project please visit the wiki
