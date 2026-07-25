-> Main

=== Main ===
Hello, I'm assistant <color=\#F8FF30>Owin</color>. I'll be helping you in this <color=\#F8FF30>circuit maze</color> about the puzzles. #speaker : SYSTEM #portrait : owin_neutral #layout:right
-> Intro

=== Intro ===
When you find other <color=\#F8FF30>info chips</color>, don't forget to interact with them to understand the puzzles.
And...
Don't lose your way in the maze...
Did you understand? #speaker : SYSTEM #portrait : owin_happy
    + [Yes]
        -> FINISH
    + [Can you explain one more time?]
        -> Intro

=== FINISH ===
Take care... #speaker : SYSTEM #portrait : owin_happy
-> END