namespace Guestline.Domain;

// [
// {
//     "hotelId": "H1",
//     "arrival": "20240901",
//     "departure": "20240903",
//     "roomType": "DBL",
//     "roomRate": "Prepaid"
// },
// {
// "hotelId": "H1",
// "arrival": "20240902",
// "departure": "20240905",
// "roomType": "SGL",
// "roomRate": "Standard"
// }
// ]

public record HotelEntity(string Id, string Name, RoomType[] RoomTypes, Room[] Rooms);