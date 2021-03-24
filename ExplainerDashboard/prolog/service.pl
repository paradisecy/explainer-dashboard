% Author: Loizos Michael
% Date: 2017-05-20

% PRUDENS: Personalized inteRactive User-DEliberatioN Support

:- [utilities,represent,derivation,elaborate,integrate,selection].

switches([list(full), steps, track, warning, error]).

% set to working directory in which this file resides.
:- working_directory(_, '.').
:- set_prolog_stack(global, limit(1 000 000 000 000)).

% ==============================================================
reasonSRV(KBfile,Lib,X) :-
   consult(Lib),
   readFile(KBfile,KB,_VarNames),
   elaborate(KB,X,_Elab),
   format("Reason completed!").

updateSRV(KBfile,Elab,Change) :-
   readFile(KBfile,KB1,VarNames1),
   integrate(Change,Elab,KB1,KB2),
   print('KB1'),
   print(KB1),
   print('KB2'),
   print(KB2),
   saveFile(KBfile,KB2),
   readFile(KBfile,KB3),
   printKB(KB3).
%  say(['Rules: ',Rules4,'\nPrefs: ',Prefs4,'\n']).
%  format("Update completed!=",KBfile).


% updateSRV(KBfile,ElabStr,ChangeStr) :-
%    readFile(KBfile,KB1,VarNames1),
%    term_string(Elab,ElabStr,[variable_names(VNs1)]),
%    term_string(Change,ChangeStr,[variable_names(VNs2)]),
%    print('KB1'),
%    print(KB1),
%    print('KB2'),
%    print(KB2),
%    integrate(Change,Elab,KB1,KB2),
%    append([VNs1,VNs2,VarNames1],VarNames2),
%    saveFile(KBfile,KB2,VarNames2),
%    format("Update completed!").

filterSRV(KB,Xs,InOut) :-
   selection(KB,Xs,InOut,Fs),
   format("Selected: ~p\n", [Fs]),
   format("Filter completed!").

importSRV(KBfile) :-
      readFile(KBfile,KB), KB=(Rules,Prefs),
      say(['Rules: ',Rules,'\nPrefs: ',Prefs,'\n']).

exportSRV(KB,KBfile) :-
   saveFile(KBfile,KB).

% ==============================================================
