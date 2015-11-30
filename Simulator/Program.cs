﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StopGuessing.Models;
using System.IO;

namespace Simulator
{


    public class Program
    {
        private const ulong Thousand = 1000;
        private const ulong Million = Thousand * Thousand;
        private const ulong Billion = Thousand * Million;

        public async Task Main(string[] args)
        {
            await Simulator.RunExperimentalSweep((config) =>
            {
                // Scale of test
                ulong totalLoginAttempts = 1*Million; // * Million;

                // Figure out parameters from scale
                double meanNumberOfLoginsPerBenignAccountDuringExperiment = 10d;
                double meanNumberOfLoginsPerAttackerControlledIP = 100d;

                double fractionOfLoginAttemptsFromAttacker = 0.5d;
                double fractionOfLoginAttemptsFromBenign = 1d - fractionOfLoginAttemptsFromAttacker;

                double expectedNumberOfBenignAttempts = totalLoginAttempts*fractionOfLoginAttemptsFromBenign;
                double numberOfBenignAccounts = expectedNumberOfBenignAttempts/
                                                meanNumberOfLoginsPerBenignAccountDuringExperiment;

                double expectedNumberOfAttackAttempts = totalLoginAttempts*fractionOfLoginAttemptsFromAttacker;
                double numberOfAttackerIps = expectedNumberOfAttackAttempts/
                                             meanNumberOfLoginsPerAttackerControlledIP;

                // Make any changes to the config or the config.BlockingOptions within config here
                config.TotalLoginAttemptsToIssue = totalLoginAttempts;

                config.FractionOfLoginAttemptsFromAttacker = fractionOfLoginAttemptsFromAttacker;
                config.NumberOfBenignAccounts = (uint) numberOfBenignAccounts;

                // Scale of attackers resources
                config.NumberOfIpAddressesControlledByAttacker = (uint)numberOfAttackerIps;
                config.NumberOfAttackerControlledAccounts = (uint)numberOfAttackerIps;

                // Additional sources of false positives/negatives
                config.FractionOfBenignIPsBehindProxies = 0.1d;
                config.ProxySizeInUniqueClientIPs = 1000;
                config.FractionOfMaliciousIPsToOverlapWithBenign = 0.1;

                //config.BlockingOptions.NumberOfSuccessesToTrackPerIp = 15;
                //config.BlockingOptions.NumberOfFailuresToTrackPerIp = 50;
                config.BlockingOptions.Conditions = new[]
                {
                    new SimulationCondition(config.BlockingOptions, 0, "Baseline", false, false, false, false, false,
                        false, false),
                    new SimulationCondition(config.BlockingOptions, 1, "NoRepeats", true, false, false, false, false,
                        false, false),
                    new SimulationCondition(config.BlockingOptions, 2, "Cookies", true, true, false, false, false, false,
                        false),
                    new SimulationCondition(config.BlockingOptions, 3, "Credits", true, true, true, false, false, false,
                        false),
                    new SimulationCondition(config.BlockingOptions, 4, "Alpha", true, true, true, true, false, false,
                        false),
                    new SimulationCondition(config.BlockingOptions, 5, "Typos", true, true, true, true, true, false,
                        false),
                    new SimulationCondition(config.BlockingOptions, 6, "PopularThreshold", true, true, true, true, true,
                        true, false),
                    new SimulationCondition(config.BlockingOptions, 6, "PunishPopularGuesses", true, true, true, true,
                        true, true, true)
                };
                
        // Blocking parameters
        // Make typos almost entirely ignored
        config.BlockingOptions.PenaltyMulitiplierForTypo = 0.1d;
            });



        }
    }
}
