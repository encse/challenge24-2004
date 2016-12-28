package y2016.R1A.C

import cmn._

import scala.annotation.tailrec

object BFFs extends GcjSolver() {

  override def solve(fetch: Fetch, out: Out): Unit = {

    val n = fetch(PInt)

    class Node(val iNode: Int) {
      var nodeNext: Node = null
      var iCircle: Int = Int.MaxValue
      var fOnCircle = false
      var d: Int = -1
      var nodeCircle: Node = null
      var maxdTail = 0
      var length = -1


      override def toString = s"Node(nodeNext=${Option(nodeNext).map(_.iNode).orNull}, iCircle=$iCircle, fOnCircle=$fOnCircle, d=$d, nodeCircle=${Option(nodeCircle).map(_.iNode).orNull}, maxdTail=$maxdTail, length=$length, iNode=$iNode)"
    }

    val rgnode = for(i <- 0 until n)yield new Node(i+1)

    for ((iNext, node) <- fetch(PList(PInt)).zip(rgnode)) {
      node.nodeNext = rgnode(iNext - 1)
    }

    def fillLength(nodeStart: Node, length: Int): Unit = {
      for (node <- nodeStart.cond(_.length == -1).next(_.nodeNext)) {
        node.length = length
      }
    }

    def markOnCircle(nodeStart: Node): Unit = {
      var node = nodeStart
      var length = 0
      while (!node.fOnCircle) {
        node.fOnCircle = true
        node = node.nodeNext
        length += 1
      }
      fillLength(nodeStart, length)
    }

    def markCircle(nodeStart: Node, iCircle: Int, fMark: Boolean): Unit = {
      var node = nodeStart

      while (true) {
        if (node.iCircle == iCircle) {
          if(fMark)
            markOnCircle(node)
          return
        }
        else if (node.iCircle < iCircle) {
          markCircle(node, node.iCircle, false)
          return
        } else {
          node.iCircle = iCircle
        }
        node = node.nodeNext
      }
    }

    var iCircleNext = 0
    for(node <- rgnode ) {
      if (node.iCircle >= iCircleNext) {
        markCircle(node, iCircleNext, true)
        iCircleNext = node.iCircle + 1
      } else {

      }
    }

    def fillD(node: Node): Unit = {
      if (node.d != -1)
        return
      if (node.fOnCircle) {
        node.d = 0
        node.nodeCircle = node
        return
      }

      fillD(node.nodeNext)
      node.d = node.nodeNext.d + 1
      node.nodeCircle = node.nodeNext.nodeCircle
      node.nodeCircle.maxdTail = Math.max(node.nodeCircle.maxdTail, node.d)
    }

    rgnode.foreach(node => fillD(node))

    val maxLength = rgnode.filter(_.fOnCircle).map(_.length).max
    val line = (for(node <- rgnode if node.fOnCircle && node.length == 2) yield 2 + node.maxdTail + node.nodeNext.maxdTail).sum / 2

    out(Math.max(line, maxLength))
  }
}
