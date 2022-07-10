using Xunit;
using files;
using System;
public class fileTEST{
    [Theory]
    [InlineData(@"<name>whatever</name><fullName>/raiden/metal/gear.exe</fullName>")]
    [InlineData(@"<Guts>CLANG</Guts><fullName>Berserk whatever idk </fullName><ziemniak>123213.513</ziemnaik>")]
    public void checkInfoSuccess(string info){
        // arrange  && act 
        var match = file.RegexIHateU(info,file.Pattern.info);
        //assert 
        Assert.Equal(info,match.Value);

    }
    [Theory]
    [InlineData(@"<whatever>failed<whatever>")]
    [InlineData(@"<ziemniak><ziemniak></ziemniak></ziemniak>")]
    [InlineData(@"<ziemniaczek>Tatakae<</ziemniaczek>")]
    public void checkInfoFailed(string info){
        //arrange && act && assert 
        Assert.Throws<ArgumentException>(() => file.RegexIHateU(info,file.Pattern.info) );
    }

    [Theory]
    [InlineData(@"<whatever>turtle</whatever>","<whatever>turtle</whatever>","whatever","turtle")]
    [InlineData(@"<fullName>/home/itam/factorio/bin/factorio.exe</fullname><ignore>Just ignore next segment :D</ignore>",
    "<fullName>/home/itam/factorio/bin/factorio.exe</fullname>",
    "fullName",
    "/home/itam/factorio/bin/factorio.exe")]
    public void getInfoSegmentSuccess(string info,string segment,string attrname,string value){
        // arrange & act 
        string resegment = file.RegexIHateU(info,file.Pattern.infoSegment).Value;
        string reattrname = file.RegexIHateU(resegment,file.Pattern.attrname).Value;
        string revalue = file.RegexIHateU(resegment,file.Pattern.valueIDK).Value;
        
        //assert
        Assert.Equal(segment,resegment);
        Assert.Equal(attrname,reattrname);
        Assert.Equal(value,revalue);

    }

    [Theory]
    [InlineData(@"<whatever turtle </whatever")]
    [InlineData(@"<whatever>aaa<</whatever>")]
    public void getInfoSegmentFailed(string info){
        

        // act && arrange && assert
        Assert.Throws<ArgumentException>(() => file.RegexIHateU(info,file.Pattern.infoSegment).Value);
    }

    [Theory]
    [InlineData(@">/home/.the/only/thing/i/know/for/real/raiden.exe<",
    @"/home/.the/only/thing/i/know/for/real/raiden.exe")]
    [InlineData(@">/homek/.the/only/thing/i/know/for/real/raiden.exexdxddxd<",
    @"/homek/.the/only/thing/i/know/for/real/raiden.exexdxddxd")]

    public void getfullNameSuccess(string info,string excepted){
        //  act & arrange 
        var refullName = file.RegexIHateU<string>(info,"fullName").ToString();
        //assert
        Assert.Equal(excepted,refullName);

    }

    [Theory]
    [InlineData(@">/home/itam//ninnin<")]
    [InlineData(@">home/itam/ninnin<")]
    [InlineData(@">/home/itam//ninnin/<")]
    public void getfullNameFailed(string info){
        // act && arrange && assert
        Assert.Throws<ArgumentException>(()=>file.RegexIHateU<file.Pattern>(info,file.Pattern.fullName).ToString());
    }

    [Theory]
    [InlineData(@"<maslo>/home/itam/whatever</maslo>","/home/itam/whatever")]
    public void getlocalPathSuccess(string info,string expected){

        //act && arrange
        var result = file.RegexIHateU<string>(info,"localPath").ToString();
        // assert 
        Assert.Equal(expected,result);
    }

    [Theory]
    [InlineData(@"<eren>D41D8CD98F00B204E9800998ECF8427E</eren>","D41D8CD98F00B204E9800998ECF8427E")]
    [InlineData(@"<the>AB5E2C19EAE9C8B318A52D8595C16010</the>","AB5E2C19EAE9C8B318A52D8595C16010")]
    public void getHashSuccess(string info,string expected){
        // act && arrange
        var result = file.RegexIHateU<string>(info,"hash");
        // assert
        Assert.Equal(expected,result.Value);
    }

    [Theory]
    [InlineData(@"<head>123456789ABCDEFGF</head>")]
    [InlineData(@"<head>FRYTKA5</head>")]
    [InlineData(@"<head>hereiamdirtyandfacelesswaitingtoheedyourinstructiononmywoninvinciblewarrioriamthewindofthedestruction</head>")]
    [InlineData(@"<head>TOALLMENWHOBENDt")]
    public void getHashFailed(string info){
        //arrange && act && assert
        Assert.Throws<ArgumentException>(() => file.RegexIHateU<string>(info,"hash"));
    }

    [Theory]
    [InlineData(@"<wTimeTicks>637928261827588277</wTimeTicks>","637928261827588277")]
    [InlineData(@"<wTimeTicks>637928261940023780</wTimeTicks>","637928261940023780")]
    [InlineData(@"<wTimeTicks>637928262012691842</wTimeTicks>","637928262012691842")]
    public void getwTimeTicksSuccess(string info,string expected){
        //  arrange && act 
        var result = file.RegexIHateU<string>(info,"wTimeTicks").Value;
        //  assert 
        Assert.Equal(expected,result);
    }

    [Theory]
    [InlineData(@"<wTimeTicks>637928261827a588277</wTimeTicks>")]
    [InlineData(@"<wTimeTicks>637928261940 023780</wTimeTicks>")]
    [InlineData(@"<wTimeTicks>6379282620126[91842</wTimeTicks>")]
    public void getwTimeTicksFailed(string info){
        // act arrange and assert 
        Assert.Throws<ArgumentException>(() => file.RegexIHateU<string>(info,"wTimeTicks"));
    }
    
    [Theory]
    [InlineData(@"<name>HomeWork</name>","HomeWork")]
    [InlineData(@"<name>factorio.exe</name>","factorio.exe")]
    [InlineData(@"<name>anime cat girl.jpg</name>","anime cat girl.jpg")]
    public void getNameSuccess(string info,string expected){
        // arrange and act 
        var result = file.RegexIHateU<string>(info,"name").Value;
        // assert
        Assert.Equal(expected,result);
    }

    [Theory]
    [InlineData(@"<name>/home/itam/HomeWork/animecatgirluwu.raw</name>")]
    [InlineData(@"<name>\\MyDressUpDariling.exe\folder</name")]
    public void getNameFailed(string info){
        //arrange , act and assert
        Assert.Throws<ArgumentException>(() => file.RegexIHateU<string>(info,"name")); 
    }
}