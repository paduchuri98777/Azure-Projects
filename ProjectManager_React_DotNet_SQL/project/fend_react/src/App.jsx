import React, { useEffect, useMemo, useState } from "react";
import { api } from "./api";

const emptyProject = {
  name: "",
  description: "",
  status: "Active"
};

const emptyTask = {
  projectId: "",
  title: "",
  description: "",
  status: "Todo"
};

function App() {
  const [projects, setProjects] = useState([]);
  const [tasks, setTasks] = useState([]);
  const [projectForm, setProjectForm] = useState(emptyProject);
  const [taskForm, setTaskForm] = useState(emptyTask);
  const [editingProject, setEditingProject] = useState(null);
  const [editingTask, setEditingTask] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");

  async function loadData() {
    try {
      setLoading(true);
      const [projectData, taskData] = await Promise.all([
        api.getProjects(),
        api.getTasks()
      ]);
      setProjects(projectData);
      setTasks(taskData);
      setMessage("");
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadData();
  }, []);

  const stats = useMemo(() => ({
    projects: projects.length,
    tasks: tasks.length,
    completed: tasks.filter((t) => t.status === "Completed").length,
    active: tasks.filter((t) => t.status !== "Completed").length
  }), [projects, tasks]);

  function resetProject() {
    setProjectForm(emptyProject);
    setEditingProject(null);
  }

  function resetTask() {
    setTaskForm(emptyTask);
    setEditingTask(null);
  }

  async function saveProject(event) {
    event.preventDefault();
    try {
      if (editingProject) {
        await api.updateProject(editingProject.id, projectForm);
      } else {
        await api.createProject(projectForm);
      }
      resetProject();
      await loadData();
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    }
  }

  async function removeProject(id) {
    if (!window.confirm("Delete this project? Its tasks will also be removed.")) return;
    try {
      await api.deleteProject(id);
      await loadData();
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    }
  }

  async function saveTask(event) {
    event.preventDefault();
    try {
      const payload = {
        ...taskForm,
        projectId: Number(taskForm.projectId)
      };

      if (editingTask) {
        await api.updateTask(editingTask.id, payload);
      } else {
        await api.createTask(payload);
      }

      resetTask();
      await loadData();
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    }
  }

  async function removeTask(id) {
    if (!window.confirm("Delete this task?")) return;
    try {
      await api.deleteTask(id);
      await loadData();
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    }
  }

  function editProject(project) {
    setEditingProject(project);
    setProjectForm({
      name: project.name,
      description: project.description || "",
      status: project.status
    });
  }

  function editTask(task) {
    setEditingTask(task);
    setTaskForm({
      projectId: String(task.projectId),
      title: task.title,
      description: task.description || "",
      status: task.status
    });
  }

  function projectName(id) {
    return projects.find((p) => p.id === id)?.name || "Unknown";
  }

  return (
    <div className="app">
      <header className="header">
        <div>
          <h1>Project Manager</h1>
          <p>React + ASP.NET Core + SQL Server</p>
        </div>
        <button onClick={loadData}>Refresh</button>
      </header>

      {message && (
        <div className="message">
          {message}
          <button onClick={() => setMessage("")}>×</button>
        </div>
      )}

      <section className="stats">
        <div className="stat"><span>Projects</span><strong>{stats.projects}</strong></div>
        <div className="stat"><span>Total Tasks</span><strong>{stats.tasks}</strong></div>
        <div className="stat"><span>Active Tasks</span><strong>{stats.active}</strong></div>
        <div className="stat"><span>Completed</span><strong>{stats.completed}</strong></div>
      </section>

      <main className="grid">
        <section className="card">
          <div className="card-title">
            <h2>{editingProject ? "Edit Project" : "Add Project"}</h2>
          </div>

          <form onSubmit={saveProject}>
            <label>Project Name</label>
            <input
              required
              value={projectForm.name}
              onChange={(e) => setProjectForm({ ...projectForm, name: e.target.value })}
            />

            <label>Description</label>
            <textarea
              value={projectForm.description}
              onChange={(e) => setProjectForm({ ...projectForm, description: e.target.value })}
            />

            <label>Status</label>
            <select
              value={projectForm.status}
              onChange={(e) => setProjectForm({ ...projectForm, status: e.target.value })}
            >
              <option>Active</option>
              <option>Completed</option>
              <option>On Hold</option>
            </select>

            <div className="actions">
              <button className="primary" type="submit">
                {editingProject ? "Update Project" : "Add Project"}
              </button>
              {editingProject && <button type="button" onClick={resetProject}>Cancel</button>}
            </div>
          </form>
        </section>

        <section className="card">
          <div className="card-title">
            <h2>{editingTask ? "Edit Task" : "Add Task"}</h2>
          </div>

          <form onSubmit={saveTask}>
            <label>Project</label>
            <select
              required
              value={taskForm.projectId}
              onChange={(e) => setTaskForm({ ...taskForm, projectId: e.target.value })}
            >
              <option value="">Select project</option>
              {projects.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>

            <label>Task Title</label>
            <input
              required
              value={taskForm.title}
              onChange={(e) => setTaskForm({ ...taskForm, title: e.target.value })}
            />

            <label>Description</label>
            <textarea
              value={taskForm.description}
              onChange={(e) => setTaskForm({ ...taskForm, description: e.target.value })}
            />

            <label>Status</label>
            <select
              value={taskForm.status}
              onChange={(e) => setTaskForm({ ...taskForm, status: e.target.value })}
            >
              <option>Todo</option>
              <option>In Progress</option>
              <option>Completed</option>
            </select>

            <div className="actions">
              <button className="primary" type="submit">
                {editingTask ? "Update Task" : "Add Task"}
              </button>
              {editingTask && <button type="button" onClick={resetTask}>Cancel</button>}
            </div>
          </form>
        </section>
      </main>

      <section className="card table-card">
        <div className="card-title">
          <h2>Projects</h2>
        </div>

        {loading ? <p>Loading...</p> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Description</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {projects.map((project) => (
                  <tr key={project.id}>
                    <td>{project.id}</td>
                    <td>{project.name}</td>
                    <td>{project.description}</td>
                    <td><span className="badge">{project.status}</span></td>
                    <td>
                      <button onClick={() => editProject(project)}>Edit</button>
                      <button className="danger" onClick={() => removeProject(project.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>

      <section className="card table-card">
        <div className="card-title">
          <h2>Tasks</h2>
        </div>

        {loading ? <p>Loading...</p> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Project</th>
                  <th>Title</th>
                  <th>Description</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {tasks.map((task) => (
                  <tr key={task.id}>
                    <td>{task.id}</td>
                    <td>{projectName(task.projectId)}</td>
                    <td>{task.title}</td>
                    <td>{task.description}</td>
                    <td><span className="badge">{task.status}</span></td>
                    <td>
                      <button onClick={() => editTask(task)}>Edit</button>
                      <button className="danger" onClick={() => removeTask(task.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}

export default App;