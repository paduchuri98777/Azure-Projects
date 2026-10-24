namespace ITServiceDesk.API.Models;
public class Ticket {
 public int Id{get;set;} public string Title{get;set;}=""; public string Description{get;set;}="";
 public int CategoryId{get;set;} public string Priority{get;set;}="Medium"; public string Status{get;set;}="Open";
 public int CreatedById{get;set;} public int? AssignedToId{get;set;} public DateTime CreatedAt{get;set;} public DateTime UpdatedAt{get;set;}
}