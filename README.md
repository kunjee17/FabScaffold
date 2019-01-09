# Fabulous Scaffold Application

## Packages Included
- Fabulous.Core for doing all the Fabulous stuff
- Fabulous.LiveReload : Not using is currently as it is not working for multiple files
- Akavache for storing data with support of in memory data in debug mode and local storage in release mode
- Bogus for generating data in debug more

## Architecture
- Divided into Authentication and Application modules
- While Authentication will take Login and SignUp as Tab Page
- Application will take multiple pages as part of Application
- All pages are stacked as part of Navigation page from Xamarin Forms

## Issues solved
- List will not refresh every time you come back to page
- Page data will be updated every time you come back to page
- Extendable architecture to allow big business application to graw in **Lego** fashion.
- Every data access is pushed to Thread pool for maximum performance. *Be careful you might need to switch back to UI thread of UI thread related stuff. *
- UI it totally non-blocking.
- Data generation is happening in async parallel. *Not required but provide good example of how things can be async and parallel.*
- Entry cell text change is working smooth on Android also. *Thanks to debounce*.


## Contribute
- All contribution are always welcome. You can help this app to make is better and faster.
- There are few obvious UI related things I like in this app to happen, animation while page changes, icons, neat arrangement etc.
- Notification library works well with this app but I haven't added it keep it simple.
- Xamarin Essential also works fine but haven't found good and simple use case to include.
