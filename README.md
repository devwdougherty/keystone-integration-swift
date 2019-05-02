# Keystone Integration Swift Object Storage

This sample was built based on [SwiftBox repository by suniln](https://github.com/suniln/SwiftBox).

A basic implementation of integration between OpenStack services: KeyStone (authentication) and Swift (Object Storage API). The project uses [RestSharp REST Client](http://restsharp.org/) to consume Swift API.

## API SWIFT Methods Implanted
- POST object
- GET object
- DELETE list of objects

## Requirements
- On NuGet Packages install RestSharp (this project use 106.6.9v).
- An OpenStack (devstack or openstack) service up.

## Environment
- [KeyStone Identity API v3](https://developer.openstack.org/api-ref/identity/v3)
- [Swift Object Storag API v1](https://docs.openstack.org/swift/latest/api/object_api_v1_overview.html)


