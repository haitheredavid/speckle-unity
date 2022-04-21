

# Connector Unity

[![Twitter Follow](https://img.shields.io/twitter/follow/SpeckleSystems?style=social)](https://twitter.com/SpeckleSystems) [![Community forum users](https://img.shields.io/discourse/users?server=https%3A%2F%2Fdiscourse.speckle.works&style=flat-square&logo=discourse&logoColor=white)](https://discourse.speckle.works) [![website](https://img.shields.io/badge/https://-speckle.systems-royalblue?style=flat-square)](https://speckle.systems) [![docs](https://img.shields.io/badge/docs-speckle.guide-orange?style=flat-square&logo=read-the-docs&logoColor=white)](https://speckle.guide/dev/)





## Introduction

This is my personal fork of the speckle-unity connector. I'm still actively developing this project, so please use with caution. 

Sending data is pretty hacky at this point, so I would only rely on receiving data in this connector. 

This connector uses [Speckle .NET SDK](https://github.com/specklesystems/speckle-sharp).  

![unity](https://user-images.githubusercontent.com/2679513/108543628-3a83ff00-72dd-11eb-8792-3d43ce54e6af.gif)


## RoadMap

There are some tasks I have setup to help make this connector more accessible to all types of unity-users. There is plenty to be added to this roadmap, but here are the main things I have cooking.

#### Operations based

- [x] Build out basic components for sending and receiving
- [ ] Build out more convenient methods for sending and receiving from speckle  
- [ ] Build out stream GUI 
- [ ] Main connector manager
- [ ] Setup up tests for all operation calls

#### Object based
- [x] Create component based converters so users can have more flexibility with how conversions work
- [ ] Handle tree hierarchy conversion back to speckle  
- [ ] Support Point cloud conversions
- [ ] Support BIM conversions
- [ ] Separate component conversion package 
 

## Documentation

More comprehensive developer documentation can be found in the [Speckle Docs website](https://speckle.guide/dev/).


## Developing & Debugging

We encourage everyone interested to debug / hack /contribute / give feedback to this project.

### Requirements

- Unity (we're currently testing with 2020+)
- A Speckle Server running (more on this below)
- Speckle Manager (more on this below)



### Dependencies

All dependencies to Speckle Core have been included compiled in the Asset folder until we figure out how to best reference Core.

The GraphQL library has been recompiled with a fix for Unity, see https://github.com/graphql-dotnet/graphql-client/issues/318 for more info.



### Getting Started 🏁

Following instructions on how to get started debugging and contributing to this connector.


#### Server

This connector relies on having the Speckle Manager installed. This will link up your accounts and servers that you want to access in the connector

#### Accounts

The connector itself doesn't have features to manage your Speckle accounts, this functionality has been delegated to the Speckle Manager desktop app.

You can install an alpha version of it from: [https://speckle-releases.ams3.digitaloceanspaces.com/manager/SpeckleManager%20Setup.exe](https://speckle-releases.ams3.digitaloceanspaces.com/manager/SpeckleManager%20Setup.exe)

After installing it, you can use it to add/create an account on the Server.



### Debugging

Open your IDE and click "Attach to Unity and Debug".



### Questions and Feedback 💬

Hey, this is work in progress, I'm sure you'll have plenty of feedback, and we want to hear all about it! Get in touch with us on [the forum](https://discourse.speckle.works)! 



## Contributing

Please make sure you read the [contribution guidelines](.github/CONTRIBUTING.md) for an overview of the best practices we try to follow.



## Community

The Speckle Community hangs out on [the forum](https://discourse.speckle.works), do join and introduce yourself!



## License

Unless otherwise described, the code in this repository is licensed under the Apache-2.0 License. Please note that some modules, extensions or code herein might be otherwise licensed. This is indicated either in the root of the containing folder under a different license file, or in the respective file's header. If you have any questions, don't hesitate to get in touch with us via [email](mailto:hello@speckle.systems).

