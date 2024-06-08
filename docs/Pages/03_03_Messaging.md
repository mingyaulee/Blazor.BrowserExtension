# Messaging between the contexts

Communication between the different contexts is a common requirement, the way to achieve this depends on the source and the target context of the message.

## Sending a message

The `Runtime.SendMessage` API can be used from any context to send a message to all other instances except for Content Scripts.

The `Tabs.SendMessage` API can be used from any context to send a message to all the instances in a tab, i.e. Content Scripts and extension iframes.

Any object can be passed when invoking the API, so you can just create a message wrapper to include the metadata for the message, e.g. sender, intended recipient, timestamp, message type, payload, etc.


## Receiving a message

The `Runtime.OnMessage` API can be used from any context to receive a message.

It is essential to be aware that there may be multiple instances receiving the same message, therefore you can use something to filter out noise messages, for example, having a property in the message object to indicate which context the message is intended for.

The pages with listeners will only get the message if the page is active when the message is sent.
However, for Background Worker that listens to `OnMessage` event, it will be activated if it is already idle when the message is sent.


## Continuous Messaging

The contexts can also use the `Runtime.Connect` and `Tabs.Connect` API to create a `Port` that you can use to continuously send messages to the target context.

The recipient context should then use the `Runtime.OnConnect` event to listen to port creation events and respond to messages on the `Port` object.


# Reference

- [Chrome for Developers](https://developer.chrome.com/docs/extensions/develop/concepts/messaging)
- [MDN](https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/runtime)
- [Messaging Sample Project](https://github.com/mingyaulee/Blazor.BrowserExtension.Samples/tree/main/Messaging)
