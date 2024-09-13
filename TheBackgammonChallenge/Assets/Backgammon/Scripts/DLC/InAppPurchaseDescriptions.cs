using UnityEngine;

namespace Backgammon
{
    public static class InAppPurchaseDescriptions
    {
        public static PurchaseDescription PurchaseDescriptionStruct(string productDefinition)
        {
            Debug.Log($"DESCRIPTION BY ID {productDefinition}");

            switch(productDefinition)
            {
                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle001":
                    return new PurchaseDescription("29th INBC Tournament Final", "Ziro Okiho", "Hirotaka Kato",  "[4 games]",
                                                   "SLAVIA OPEN 2022 Semi Final", "Georgios Lazaris", "Bakar Matikashvili", "[16 games]",
                                                   "WBIF World Individual Championship 2022", "Patrik Gullberg", "Honza Cerny", "[12 games]",
                                                   "World Club Championship 2021", "Kengo Nakasuka", "Martin Barkwill", "[11 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle002":
                    return new PurchaseDescription("Japan Open2022 Final", "Kawauchi Hibiki", "Moriuchi Toshiyuki", "[10 games]",
                                                   "WBIF World Individual Championship 2022", "Mislav Kovacic", "Naoki Iketani", "[16 games]",
                                                   "WBT Grand Finale Final", "Soren Larsen", "Masayuki Mochizuki", "[5 games]",
                                                   "World Club Championship 2021", "Tim Cross", "Hideaki Ueda", "[13 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle003":
                    return new PurchaseDescription("Rainer's Backgammon Live Stream #92", "Torsten Lux & Rainer Birkle", "Masayuki Mochizuki & Kazuki Yokota", "[6 games]",
                                               "WBIF World Cup 2022", "Marty Storer", "Martin Zizka", "[18 games]",
                                               "WBIF World Individual Championship 2022", "Ernst Kümin", "Joseph Russell", "[15 games]",
                                               "WBIF World Individual Championship 2022", "Giancarlo Ratto", "Bernhard Kaiser", "[6 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle004":
                    return new PurchaseDescription("29. WBIF Open", "Eva Zizkova", "Mislav Kovacic", "[7 games]",
                                                   "World Club Championship 2021 Final", "Frank Simon", "Hideaki Ueda", "[13 games]",
                                                   "World Club Championship 2021 Final", "Hideaki Ueda", "Marcus Reinhard", "[14 games]",
                                                   "World Club Championship 2021", "Stephan Hartmann", "Naoki Iketani", "[10 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle005":
                    return new PurchaseDescription("10th INBC Meijinsen", "Hideaki Ueda", "Yoshimasa Tsuruoka", "[6 games]",
                                                   "WBIF Champions Cup 2022", "Hiromichi Sugimoto", "Fernando Braconi", "[16 games]",
                                                   "WBIF World Individual Championship 2022", "Naoki Iketani", "Sam Mizutani", "[13 games]",
                                                   "WBIF World Individual Championship 2022", "Zdenek Zizka", "Rory Pascar", "[14 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle006":
                    return new PurchaseDescription("WBIF Champions Cup 2022", "Aref Alipour", "Roland Sahlén", "[7 games]",
                                                   "WBIF Champions Cup 2022", "Iosebi Menabdishvili", "Naoki Iketani", "[13 games]",
                                                   "WBIF Champions Cup 2022", "Naoki Iketani", "Honza Cerny", "[12 games]",
                                                   "WBIF World Individual Championship 2022", "Michael Weile", "Dirk Schiemann", "[14 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle007":
                    return new PurchaseDescription("2022 BGisBig! Masters", "Gaz Owen", "Rainer Birkle", "[8 games]",
                                                   "WBIF World Individual Championship 2022", "Anna Price", "Hideaki Ueda", "[11 games]",
                                                   "WBIF World Individual Championship 2022", "Takayuki Hino", "Geir Pedersen", "[16 games]",
                                                   "World Club Championship 2021 True Final", "Jürgen Schettler", "Kengo Nakasuka", "[11 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle008":
                    return new PurchaseDescription("WBIF Champions Cup 2022", "Martin-Frederic Kahn", "Zdenek Zizka", "[7 games]",
                                                   "WBIF Champions Cup 2022", "Naoki Iketani", "Saeed Nourmohammadi", "[12 games]",
                                                   "WBIF World Individual Championship 2022", "Dagfinn Snarheim", "Patrick Didisheim", "[13 games]",
                                                   "WBIF World Individual Championship 2022", "Dirk Schiemann", "Dmitriy Obukhov", "[14 games]");

                case "com.asermet.thebackgammonchallenge.tournamentmatchbundle009":
                    return new PurchaseDescription("2022 Meijin League", "Kiyokazu Nishikawa", "Daisuke Katagami", "[16 games]",
                                                   "WBGF Venice Individual Championship 2022 Final", "Patrick Didisheim", "Jörgen Granstedt", "[17 games]",
                                                   "WBIF Champions Cup 2022", "Naoki Iketani", "Rune Halvorsen", "[6 games]",
                                                   "WBIF World Individual Championship 2022", "Karl Frogner", "Nick Blasier", "[14 games]");

                    // UK MATCH BUNDLES

                case "com.asermet.thebackgammonchallenge.ukmatchbundle101":
                    return new PurchaseDescription("UKBGF League", "Michihito Kageyama", "Simon Barget", "[7 games]",
                                                   "WBIF World Club Championship 2022", "Takayuki Hino", "Reece Hodges", "[10 games]",
                                                   "WBIF World Womenâs Championship 2022 Finals", "Junko Nakamura", "Julia Hayward", "[9 games]",
                                                   "World Individual OnlineChampionship 2021", "Dmitriy Obukhov", "Chris Rogers", "[12 games]");

                case "com.asermet.thebackgammonchallenge.ukmatchbundle102":
                    return new PurchaseDescription("WBIF Online Team Championship 2021", "Neil Kazaross", "Reece Hodges", "[13 games]",
                                                   "WBIF Tour 2021 Online Tourney 3", "Claus Elken", "Steve Bibby", "[9 games]",
                                                   "WBIF World Club Championship 2022", "Hitoshi Kawaai", "Brian Lever", "[10 games]",
                                                   "WBIF World Club Championship 2022", "Julia Hayward", "Hideaki Ueda", "[13 games]");

                case "com.asermet.thebackgammonchallenge.ukmatchbundle103":
                    return new PurchaseDescription("3rd IBC Match 1", "Jon Barnes", "Kiyokazu Nishikawa", "[10 games]",
                                                   "3rd IBC Match 2", "Kiyokazu Nishikawa", "Jon Barnes", "[3 games]",
                                                   "24. WBIF Open", "Laila Leonhardt", "Chris Rogers", "[8 games]",
                                                   "UKBGF League and Cup", "Gaz Owen", "Tim Cross", "[6 games]");

                // US MATCH BUNDLES

                case "com.asermet.thebackgammonchallenge.usmatchbundle201":
                    return new PurchaseDescription("53RD BGWC - World Championship 2022", "Akiko Yazawa", "Phil Simborg", "[18 games]",
                                                   "2016 US Open Championship Division-Main Final", "Gary Bauer", "Kit Woolsey", "[19 games]",
                                                   "Slavia Open All Stars vs WBIF Dream Team 2022", "Karen Davis", "Bakar Matikashvili", "[3 games]",
                                                   "US Open Super Jackpot Final", "Ed O'Laughlin", "Jacob Rice", "[14 games]");

                case "com.asermet.thebackgammonchallenge.usmatchbundle202":
                    return new PurchaseDescription("Japan Open main Round 1", "Carol Joy Cole", "Katagami Daisuke", "[19 games]",
                                                   "Texas Backgammon Championships Final", "Phil Simborg", "Chuck Bower", "[7 games]",
                                                   "WBIF Online Team Championship 2021", "Neil Kazaross", "Reece Hodges", "[13 games]",
                                                   "WBIF Online Team Championship 2021", "Neil Kazaross", "Vesselin Kolev", "[14 games]");

                case "com.asermet.thebackgammonchallenge.usmatchbundle203":
                    return new PurchaseDescription("Las Vegas Open 2022 - Masters Jackpot", "Matt Cohn-Geier", "Kit Woolsey", "[9 games]",
                                                   "Las Vegas Open Final", "Kit Woolsey", "Yazawa Akiko", "[11 games]",
                                                   "Nordic Open Championship quarter final", "Ed O'Laughlin", "Konstantinos Mitrelis", "[20 games]",
                                                   "WBIF Champions Cup 2022", "Cuneyt Argun Genc", "Neil Kazaross", "[10 games]");
            }

            return new PurchaseDescription();
        }

        public struct PurchaseDescription
        {
            public string match1;
            public string games1;
            public string player11;
            public string player12;

            public string match2;
            public string games2;
            public string player21;
            public string player22;

            public string match3;
            public string games3;
            public string player31;
            public string player32;
            
            public string match4;
            public string games4;
            public string player41;
            public string player42;

            public PurchaseDescription (string _match1, string _player11, string _player12, string _games1,
                                        string _match2, string _player21, string _player22, string _games2,
                                        string _match3, string _player31, string _player32, string _games3,
                                        string _match4, string _player41, string _player42, string _games4)
            {
                match1 = _match1;
                player11 = _player11;
                player12 = _player12;
                games1 = _games1;

                match2 = _match2;
                player21 = _player21;
                player22 = _player22;
                games2 = _games2;

                match3 = _match3;
                player31 = _player31;
                player32 = _player32;
                games3 = _games3;

                match4 = _match4;
                player41 = _player41;
                player42 = _player42;
                games4 = _games4;
            }
        }
    }
}