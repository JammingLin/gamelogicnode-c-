%%  coding: latin-1
%%------------------------------------------------------------
%%
%% Implementation stub file
%% 
%% Target: CosTransactions_WrongTransaction
%% Source: /net/isildur/ldisk/daily_build/18_prebuild_opu_o.2016-03-14_21/otp_src_18/lib/cosTransactions/src/CosTransactions.idl
%% IC vsn: 4.4
%% 
%% This file is automatically generated. DO NOT EDIT IT.
%%
%%------------------------------------------------------------

-module('CosTransactions_WrongTransaction').
-ic_compiled("4_4").


-include("CosTransactions.hrl").

-export([tc/0,id/0,name/0]).



%% returns type code
tc() -> {tk_except,"IDL:omg.org/CosTransactions/WrongTransaction:1.0",
                   "WrongTransaction",[]}.

%% returns id
id() -> "IDL:omg.org/CosTransactions/WrongTransaction:1.0".

%% returns name
name() -> "CosTransactions_WrongTransaction".



