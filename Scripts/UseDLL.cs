using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System;

public class UseDLL
{



    // Use this for initialization
    public int readFrameCounter = 0;
    public Double[,] pos;
    public int nAtom;
    public AtomAttributes[] atoms;
    public int nFrame = 10000;

    public void InitialStep()
    {
        Debug.Log("Inside Initial Step");
        //CSharpLibrary.CSharpLibrary.la
        var lmp_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(System.IntPtr)));

        CSharpLibrary.CSharpLibrary.lammps_open_no_mpi(0, System.IntPtr.Zero, lmp_ptr);
        var deref1 = (System.IntPtr)Marshal.PtrToStructure(lmp_ptr, typeof(System.IntPtr));
        Debug.Log("Lammps Pointer: " + deref1);

        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "# package omp 40");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "# suffix omp");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "variable xx equal 20");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "variable yy equal 20");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "variable zz equal 20");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "units lj");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "boundary p p p");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "atom_style atomic");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "lattice fcc 0.8442");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "region box block 0 ${xx} 0 ${yy} 0 ${zz}");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "create_box  1 box");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "create_atoms 1 box");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "mass 1 1.0");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "velocity all create 10 87287 loop geom");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "neighbor 0.3 bin");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "neigh_modify delay 0 every 20 check no");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "pair_style lj/cut 2.5");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "pair_coeff  1 1 1.0 1.0 2.5");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "fix 1 all nve");
        CSharpLibrary.CSharpLibrary.lammps_command(deref1, "run 0");

        // Debug.Log(CSharpLibrary.CSharpLibrary.lammps_command(deref1, "run 100000 pre no post no"));
        System.IntPtr natmPtr = CSharpLibrary.CSharpLibrary.lammps_extract_global(deref1, "natoms");
        nAtom = (int)Marshal.PtrToStructure(natmPtr, typeof(int));

        Debug.Log(nAtom);

        atoms = new AtomAttributes[nAtom];

        Double[] temp = new Double[3];

        System.IntPtr[] PointerToFloat = new IntPtr[nAtom];
        System.IntPtr posPtr;

        for (int j = 0; j < nFrame; j++)
        {
            if (j > 0)
            {
                CSharpLibrary.CSharpLibrary.lammps_command(deref1, "run 1 pre no post no");
                natmPtr = CSharpLibrary.CSharpLibrary.lammps_extract_global(deref1, "natoms");
                nAtom = (int)Marshal.PtrToStructure(natmPtr, typeof(int));
            }
        
            posPtr = CSharpLibrary.CSharpLibrary.lammps_extract_atom(deref1, "x");
            Marshal.Copy(posPtr, PointerToFloat, 0, nAtom);
            

            for (int i = 0; i < nAtom; i++)
            {
                Marshal.Copy(PointerToFloat[i], temp, 0, 3);

                double[] rr = new double[3] { temp[0]/2, temp[1]/2, temp[2]/2};
                AtomAttributes at = new AtomAttributes("C", rr);
                if (j == 0)
                {
                    atoms[i] = at;
                }
                else if (j>0)
                {
                    atoms[i].framePos.Add(new Vector3((float)temp[0]/2, (float)temp[1]/2, (float)temp[2]/2));
                }
            }
            readFrameCounter++;
        }
        
        CSharpLibrary.CSharpLibrary.lammps_close(deref1);

        Marshal.FreeHGlobal(lmp_ptr);
        lmp_ptr = System.IntPtr.Zero;
    }

    
    
}