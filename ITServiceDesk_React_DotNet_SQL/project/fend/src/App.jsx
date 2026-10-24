import React,{useEffect,useMemo,useState} from "react";
import {api} from "./api";
const empty={title:"",description:"",categoryId:"",priority:"Medium",status:"Open",createdById:"",assignedToId:""};
export default function App(){
 const [tickets,setTickets]=useState([]),[users,setUsers]=useState([]),[categories,setCategories]=useState([]),[form,setForm]=useState(empty);
 const [editing,setEditing]=useState(null),[filter,setFilter]=useState("All"),[loading,setLoading]=useState(true),[message,setMessage]=useState("");
 async function load(){try{setLoading(true);const [t,u,c]=await Promise.all([api.getTickets(),api.getUsers(),api.getCategories()]);setTickets(t);setUsers(u);setCategories(c);setMessage("")}catch(e){setMessage("Error: "+e.message)}finally{setLoading(false)}}
 useEffect(()=>{load()},[]);
 const stats=useMemo(()=>({total:tickets.length,open:tickets.filter(x=>x.status==="Open").length,progress:tickets.filter(x=>x.status==="In Progress").length,resolved:tickets.filter(x=>["Resolved","Closed"].includes(x.status)).length}),[tickets]);
 const visible=filter==="All"?tickets:tickets.filter(x=>x.status===filter);
 const userName=id=>users.find(x=>x.id===id)?.name||"-",catName=id=>categories.find(x=>x.id===id)?.name||"-";
 function reset(){setForm(empty);setEditing(null)}
 function edit(t){setEditing(t.id);setForm({title:t.title,description:t.description||"",categoryId:String(t.categoryId),priority:t.priority,status:t.status,createdById:String(t.createdById),assignedToId:t.assignedToId?String(t.assignedToId):""});window.scrollTo({top:0,behavior:"smooth"})}
 async function save(e){e.preventDefault();try{const data={title:form.title,description:form.description,categoryId:Number(form.categoryId),priority:form.priority,status:form.status,createdById:Number(form.createdById),assignedToId:form.assignedToId?Number(form.assignedToId):null};editing?await api.updateTicket(editing,data):await api.createTicket(data);reset();await load()}catch(e){setMessage("Error: "+e.message)}}
 async function remove(id){if(!confirm("Delete this ticket?"))return;try{await api.deleteTicket(id);await load()}catch(e){setMessage("Error: "+e.message)}}
 return <div className="app">
  <header><div><h1>IT Service Desk</h1><p>React + ASP.NET Core + SQL Server</p></div><button onClick={load}>Refresh</button></header>
  {message&&<div className="message"><span>{message}</span><button onClick={()=>setMessage("")}>×</button></div>}
  <section className="stats">{[["Total Tickets",stats.total],["Open",stats.open],["In Progress",stats.progress],["Resolved / Closed",stats.resolved]].map(x=><div className="stat" key={x[0]}><span>{x[0]}</span><strong>{x[1]}</strong></div>)}</section>
  <section className="card"><h2>{editing?`Edit Ticket #${editing}`:"Create Support Ticket"}</h2>
   <form onSubmit={save}><div className="formgrid">
    <div><label>Title</label><input required value={form.title} onChange={e=>setForm({...form,title:e.target.value})}/></div>
    <div><label>Category</label><select required value={form.categoryId} onChange={e=>setForm({...form,categoryId:e.target.value})}><option value="">Select category</option>{categories.map(c=><option key={c.id} value={c.id}>{c.name}</option>)}</select></div>
    <div><label>Priority</label><select value={form.priority} onChange={e=>setForm({...form,priority:e.target.value})}><option>Low</option><option>Medium</option><option>High</option><option>Critical</option></select></div>
    <div><label>Status</label><select value={form.status} onChange={e=>setForm({...form,status:e.target.value})}><option>Open</option><option>In Progress</option><option>Pending</option><option>Resolved</option><option>Closed</option></select></div>
    <div><label>Created By</label><select required value={form.createdById} onChange={e=>setForm({...form,createdById:e.target.value})}><option value="">Select user</option>{users.map(u=><option key={u.id} value={u.id}>{u.name}</option>)}</select></div>
    <div><label>Assigned To</label><select value={form.assignedToId} onChange={e=>setForm({...form,assignedToId:e.target.value})}><option value="">Unassigned</option>{users.map(u=><option key={u.id} value={u.id}>{u.name}</option>)}</select></div>
   </div><label>Description</label><textarea required value={form.description} onChange={e=>setForm({...form,description:e.target.value})}/><div className="actions"><button className="primary">{editing?"Update Ticket":"Create Ticket"}</button>{editing&&<button type="button" onClick={reset}>Cancel</button>}</div></form>
  </section>
  <section className="card"><div className="toolbar"><h2>Tickets</h2><select value={filter} onChange={e=>setFilter(e.target.value)}>{["All","Open","In Progress","Pending","Resolved","Closed"].map(x=><option key={x}>{x}</option>)}</select></div>
   {loading?<p>Loading tickets...</p>:<div className="tablewrap"><table><thead><tr><th>ID</th><th>Title</th><th>Category</th><th>Priority</th><th>Status</th><th>Created By</th><th>Assigned To</th><th>Actions</th></tr></thead><tbody>
   {visible.map(t=><tr key={t.id}><td>INC-{String(t.id).padStart(4,"0")}</td><td><strong>{t.title}</strong><small>{t.description}</small></td><td>{catName(t.categoryId)}</td><td><span className={`badge p-${t.priority.toLowerCase()}`}>{t.priority}</span></td><td><span className="badge">{t.status}</span></td><td>{userName(t.createdById)}</td><td>{userName(t.assignedToId)}</td><td><button onClick={()=>edit(t)}>Edit</button><button className="danger" onClick={()=>remove(t.id)}>Delete</button></td></tr>)}
   {!visible.length&&<tr><td colSpan="8" className="empty">No tickets found.</td></tr>}</tbody></table></div>}
  </section>
 </div>
}