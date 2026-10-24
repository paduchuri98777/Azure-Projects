const API_BASE = import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api, https://backenditticket-dqhvgbepetbeacga.westus3-01.azurewebsites.net/api/tickets/api";
async function request(url, options={}) {
  const response = await fetch(`${API_BASE}${url}`, {headers:{"Content-Type":"application/json",...(options.headers||{})},...options});
  if (!response.ok) { const message=await response.text(); throw new Error(message || `Request failed: ${response.status}`); }
  if (response.status===204) return null;
  return response.json();
}
export const api = {
  getTickets:()=>request("/tickets"), getTicket:(id)=>request(`/tickets/${id}`),
  createTicket:data=>request("/tickets",{method:"POST",body:JSON.stringify(data)}),
  updateTicket:(id,data)=>request(`/tickets/${id}`,{method:"PUT",body:JSON.stringify(data)}),
  deleteTicket:id=>request(`/tickets/${id}`,{method:"DELETE"}),
  getUsers:()=>request("/users"), getCategories:()=>request("/categories")
};