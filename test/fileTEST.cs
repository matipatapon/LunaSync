using Xunit;
using files;
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
        // arrange && act 
        var match = file.RegexIHateU(info,file.Pattern.info);
        //assert 
        Assert.Equal(false,match.Success);
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
        // act & arrange
        string resegment = file.RegexIHateU(info,file.Pattern.infoSegment).Value;

        //  assert
        Assert.Equal("",resegment);
    }

    
    
}