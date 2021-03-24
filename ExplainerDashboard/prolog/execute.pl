% Author: Loizos Michael
% Date: 2017-05-20

% PRUDENS: Personalized inteRactive User-DEliberatioN Support

:- [utilities,represent,derivation,perceive,elaborate,integrate,supervise,coaching,selection].

switches([list(full), steps, track, warning, error]).

% set to working directory in which this file resides.
:- working_directory(_, '.').
:- set_prolog_stack(global, limit(1 000 000 000 000)).

% ==============================================================

% autodidact: specifying contexts or typical features; use projection learning.
% supervising: offering an expected inference/action; use supervised learning.
% coaching: reacting to the produced derivations and explaining the reaction.
% instructing: directly changing the knowledge base; essentially programming.

% ==============================================================

ex1 :- run('data/example.kb',_,_,infer([a,-z])).
ex2 :- run('data/exponential.kb',_,_,infer([x])).
ex3 :- run('data/cycles.kb',_,_,infer([a,d])).
ex4 :- run('data/fibonacci.kb',_,_,infer([bound(100),fib(1,1),fib(2,1)])).
ex5 :- run('data/temporal.kb',_,_,infer([bird@0,clap@1,penguin@3])).
ex6 :- run('data/example.kb',_,_,learn(supervised)).
ex7 :- run('data/actions.kb',_,_,infer([a,want_to_schedule_meeting(19,2)])).
ex8 :- run('data/jenny.kb',_,_,infer([])).

calendar([
    time((2018,01,29,10,25)),
    entry(141,'meeting with Jake',((2018,01,29,11,00),(2018,01,29,12,00))),
    entry(271,'meeting with John',((2018,01,30,11,00),(2018,01,30,12,00))),
    entry(314,'call: Jane & John',((2018,01,30,13,00),(2018,01,30,14,00)))
]).

ex9(ExtraContext) :-
    calendar(Cal), append(ExtraContext,Cal,Context),
    run('data/julie.kb','data/julie.pl',_,infer(Context)).

ex90 :- ex9([]).
ex91 :- ex9([before(lunch)]).

ex92 :- ex9([quit(goodbye)]).

ex93 :- ex9([delete, title('John')]).
ex94 :- ex9([delete, before(lunch)]).

ex95 :- ex9([schedule, urgent]).
ex96 :- ex9([schedule, title('coffee time'), after(lunch)]).

run(KBfileIn,Libfile,KBfileOut,Mode) :-
   if(ground(Libfile),consult(Libfile)),
   if(ground(KBfileIn),readFile(KBfileIn,KB0),KB0=([],[],[])),
   caseDo(Mode, [
      (learn(autodidact), (true)),
      (learn(supervised), (supervisor(KB0,[],KB1))),
      (learn(coached), (coach(KB0,[],KB1))),
      (learn(instructed), (true)),
      (infer(X), (elaborate(KB0,X,KB1))),
      (offer(Offers,Mode,X), (persuador(Offers,Mode,KB0,X,[]))),
      (retain(XsFile,InOut), (selector(KB0,XsFile,InOut,_)))
   ]),
   if(ground(KBfileOut),saveFile(KBfileOut,KB1)).

% ==============================================================
