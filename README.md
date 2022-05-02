

# Connector Unity

[![Twitter Follow](https://img.shields.io/twitter/follow/SpeckleSystems?style=social)](https://twitter.com/SpeckleSystems) [![Community forum users](https://img.shields.io/discourse/users?server=https%3A%2F%2Fdiscourse.speckle.works&style=flat-square&logo=discourse&logoColor=white)](https://discourse.speckle.works) [![website](https://img.shields.io/badge/https://-speckle.systems-royalblue?style=flat-square)](https://speckle.systems) [![docs](https://img.shields.io/badge/docs-speckle.guide-orange?style=flat-square&logo=read-the-docs&logoColor=white)](https://speckle.guide/dev/)


## Introduction

This is my personal fork of the speckle-unity connector. I'm still actively developing this project, so please use with caution. 

This connector uses [Speckle .NET SDK](https://github.com/specklesystems/speckle-sharp). If you are new to Speckle I recommend looking at their developer guides on the [Speckle Docs website](https://speckle.guide/dev/) and/or jumping over and joining their community on [discourse](https://discourse.speckle.works).


![unity](https://user-images.githubusercontent.com/2679513/108543628-3a83ff00-72dd-11eb-8792-3d43ce54e6af.gif)


## RoadMap

There are some tasks I have setup to help make this connector more accessible to all types of unity-users. There is plenty to be added to this roadmap, but here are the main things I have cooking.

#### ~Operations based~ 

- [x] ~Setup object for sending~
- [x] ~Setup object for receiving~
- [x] ~Setup a Speckle Stream wrapper as scriptable obj~
- [x] ~Gather and store all active converters~ 
- [x] ~Setup some sort of connector manager~

#### GUI Stuff
- [ ] Speckle Stream scriptable obj
- [x] ~Receiver~ 
- [x] ~Sender~
- [x] ~Stream preview~
- [x] ~Speckle Connector~ 

#### Object based
- [x] Create component based converters so users can have more flexibility with how conversions work
- [ ] Handle tree hierarchy for sending to speckle 
- [ ] Handle tree hierarchy for receiving from speckle
- [x] ~Mesh converter~
- [x] ~Line converter~
- [x] ~View3d converter~ 
- [ ] Brep converter
- [ ] Point cloud converter
- [ ] Point converter
- [ ] Default display converter
- [ ] BIM properties properly stored
 
 #### Rando
 - [ ] Test scene 
 - [ ] Repo notes on how to use
 - [ ] Some crispy visuals 
 - [ ] Buffer Display Mesh load


 #### tbd
- [ ] Setting selection for sending
- [ ] Compiling all converters on build
- [ ] Setup up tests for all operation calls
- [ ] Separate component conversion package

### Note!

Sending data is pretty hacky at this point, so I would only rely on receiving data in this connector. 


### Requirements

- Unity (we're currently testing with 2021.3)
- An active speckle stream on a server
- [Speckle Manager](https://speckle.guide/#speckle-manager)


## Contributing

Please make sure you read the [contribution guidelines](.github/CONTRIBUTING.md) for an overview of the best practices we try to follow.


## License

Unless otherwise described, the code in this repository is licensed under the Apache-2.0 License. Please note that some modules, extensions or code herein might be otherwise licensed. This is indicated either in the root of the containing folder under a different license file, or in the respective file's header. If you have any questions, don't hesitate to get in touch with us via [email](mailto:hello@speckle.systems).

