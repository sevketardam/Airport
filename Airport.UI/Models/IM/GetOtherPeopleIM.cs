using System.Collections.Generic;

namespace Airport.UI.Models.IM;

public class GetOtherPeopleIM
{
    public List<GetOtherPeople> Others { get; set; }
}

public class GetOtherPeople
{
    public string OthersName { get; set; }
    public string OthersSurname { get; set; }
}
