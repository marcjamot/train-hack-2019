using System.Collections.Generic;

public class TrafikverketResponse<T> {
    public Response<T> Response { get; set; }
}

public class Response<T> {
    public T Result { get; set;}
}

public class TrainAnnouncementResponse {
    public IEnumerable<TrainAnnouncement> TrainAnnouncement { get; set; }
}

public class TrainMessageResponse {
    public IEnumerable<TrainMessage> TrainMessage { get; set; }
}