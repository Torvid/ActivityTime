I was disappointed with ActivityWatch, and so made this thing over a weekend. I plan to maintain it for a while to get it stable for myself and whoever wants to try it.

things that upset me:
- General bad/glitchy UX, relying on the web browser, slowness.
- Logging window titles is useless, as they change arbitraraly.
- Logging the exe name is also not good, a program like this should come with a massive library of common activities that have pretty-printed names.
- Figuring out what domain the user is browsing should NOT require me to install a browser plugin.
- Also, the kicker, this is not complicated software. Finding what window the user has focused and logging it disk is basically trivial.

Here's a stab at something nicer in less than 1000 lines of C#.