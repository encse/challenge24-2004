package y2016.APACA.C

import cmn._

import scala.collection.immutable._
import scala.util.control.Breaks._
import scala.collection.mutable

object GCampus extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    class Node(val i: Int) {
      val edges = mutable.ArrayBuffer[(Node, BigInt)]()
    }

    val List(cnode, cedge) = fetch(PList(PInt))

    val rgnode = for (inode <- 0 until cnode) yield new Node(inode)
    val rgedge = for (iedge <- 0 until cedge) yield {
      val List(inode1, inode2, length) = fetch(PList(PInt))
      val node1 = rgnode(inode1)
      val node2 = rgnode(inode2)
      (node1, node2, length)
    }

    for ((node1, node2, length) <- rgedge) {
      node1.edges.append((node2, length))
      node2.edges.append((node1, length))
    }

    for (((node1, node2, edgeLength), iedge) <- rgedge.zipWithIndex) {
      val mpdByNode = mutable.Map[Node, BigInt]()
      def setd(node: Node, d: BigInt) = {
        mpdByNode.get(node) match {
          case None => mpdByNode(node) = d
          case Some(dOld) => if (dOld > d) mpdByNode(node) = d
        }
      }

      setd(node1, 0)
      val minD = astar[Node, BigInt](Some(node1), _ eq node2, nodeFrom => {
        val dFrom = mpdByNode(nodeFrom)
        for ((nodeTo, length) <- nodeFrom.edges) {
          setd(nodeTo, length + dFrom)
        }
        nodeFrom.edges.map(_._1)
      }, mpdByNode(_)).get._2

      if(minD < edgeLength) {
        out(NewLine)
        out(iedge)
      }
    }
  }
}
