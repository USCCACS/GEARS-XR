# GEARS-XR: Collaborative Mixed Reality Platform for Scientific Visualization

<div align="center">
  <img src="https://github.com/USCCACS/GEARS-XR/blob/master/Resources/fig1.png" width=60%><br><br>
</div>


## Summary
Visualization plays a critically important role in modern scientific research project. It is a very powerful tool to express results from observation and experiments, and visually provides scientists with an intuitive overview of the results. So any research project would not go without visualization, like plotting graph and it is often the case that scientists plots data in many different ways with many different axes to extract some meaningful phenomena. Although a simple graph can only express data with two axes, thanks to the development of computer graphics techniques, scientists can also easily visualize data on 3D space by using visualization software. However, as long as plotted on a flat display, data are reduced onto two dimensional and scientists could miss some information inherent in the data, especially in the field targeting at objects with complex geometries or a large number of entities, like material science.

## Environment
We used “Acer Windows Mixed Reality Headset with Motion Controllers,” which has 1440x1440 resolution and 90 fps at maximum, as a head-mounted display. Each users’ headset is connected to a gaming laptop (Predator 17 X, GX-792). Speaking of software aspect, we developed all the software on the Unity 2017 game development platform, using Microsoft Mixed Reality Toolkit as a library. Concerning the networking part, our system highly depends on UNET; networking APT provided from Unity.

## Pipeline
GEARS-XR consist of three components, LAMMPS interface, multi-user component, and the rendering component. LAMMPS is a molecular dynamics simulator library, which is widely used in material science research. As is often the case in material science, a large number of atoms are involved in LAMMPS simulations. Different from any other applications, e.g., games or movies, scientific visualization require strict criteria for preciseness, which makes multi-user component and rendering component non-trivial technical building blocks.

# Sample Use Cases

## Interactive Viewing
The most straightforward application of our application is interactive viewing of pre- computed results. To demonstrate this aspect of GEARS, we use the results from precomputed atom MD simulation. The position of every molecule is calculated at each step. We isolate one step, or frame, of this simulation and import their coordinates to be rendered by our pipeline.
This aspect provides a quick, straightforward outlet for immediate visualization of data from HPC simulations or structure files like those found on the RCS Protein Data Bank. This process has been expanded to create multiple scenes, each containing the results from a different time step. Then the simulation could be replayed dynamically frame by frame.

## Real-time Simulation

Once a simulation of interest has been coded, it still needs to be incorporated with the engine’s rendering mechanics. This relation can be programmed as the user sees fit. However, to minimize the amount of coding necessary, we present two methods by which simulations can be run and rendered in real-time. One involves running a timestep on a frame update call, then updating particles states on the same frame. We call this method, Run-and-Render. This method offloads the timestep computation onto a new thread, while the main game thread, responsible for handling the render state of the engine, maintains the simulation state of the previous timestep. When the timestep is finished computing, the game thread can either appropriately update the particle states or store them for rendering later while another thread continues to calculate future timesteps. We call this method Render-when-Ready.

# Demos
## Application Demo
 [![AppDemo](https://img.youtube.com/vi/cX5vcuuTM88/0.jpg)](https://www.youtube.com/watch?v=cX5vcuuTM88)
 
## Graphene Folding Demo
This demonstration shows the capability of our application to visualize series of data frames interactively

[![AppDemo](https://img.youtube.com/vi/ZSGiU1FP5w/0.jpg)](https://www.youtube.com/watch?v=sZSGiU1FP5w)

