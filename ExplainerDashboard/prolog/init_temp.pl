:- initialization main.
main :- 
    working_directory(_, '<dir>'),
    consult('service.pl'),
    reasonSRV('<kb>','<kb>',[<context>]),
    halt().